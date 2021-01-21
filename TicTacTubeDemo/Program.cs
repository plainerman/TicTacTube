using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Reflection;
using log4net;
using log4net.Config;
using NYoutubeDL.Helpers;
using TagLib;
using Telegram.Bot.Types;
using TicTacTubeCore.Executors;
using TicTacTubeCore.Executors.Events;
using TicTacTubeCore.Genius.Processors.Media.Songs;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Processors.Logical;
using TicTacTubeCore.Processors.Media.Songs;
using TicTacTubeCore.Schedulers.Events;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Sources.Files.Comparer;
using TicTacTubeCore.Telegram.Schedulers;
using TicTacTubeCore.YoutubeDL.Sources.Files.External;
using File = System.IO.File;
using TicTacTubeCore.Processors.Filesystem;

namespace TicTacTubeDemo
{
	public class Program
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

		private static void Main(string[] args)
		{
			var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
			XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

			var scheduler = new TelegramScheduler(new Executor(8, new ExternalSourceComparer()),
				File.ReadAllText("telegram.token"));
			var pipelineBuilder = new DataPipelineBuilder();

			var extractor = new SongInfoExtractor(false);

			GeniusSongInfoFetcher genius = null;
			try {
				string apiKey = null;
				if (File.Exists("genius.token")) apiKey = File.ReadAllText("genius.token");

				genius = new GeniusSongInfoFetcher(apiKey);
			}
			catch (ArgumentException) {} // will be thrown if no token could be found

			if (genius == null) {
				Log.Warn("Neither a genius.token file nor the corresponding environmental variable has been set. -Skipping genius");
			}

			var merger = new SongInfoMerger(); // a class that can merge multiple SongInfo instances

			pipelineBuilder.Append(new LambdaProcessor(source =>
			{
				// load already stored ID3-tags (if any) from the specified file
				var originalInfo = SongInfo.ReadFromFile(source.FileInfo.FullName);

				// try to parse the filename and extract the title, and artist(s)
				var parsedInfo = extractor.ExtractFromString(source.FileName);
				// merge the metadata but ensure that they are (probably) identical
				// greedy ensures that null values are overridden
				originalInfo = merger.Merge(originalInfo, parsedInfo, greedy: true);

				if (genius != null) { // if a genius client is available
					// search for the info on genius
					parsedInfo = genius.ExtractAsyncTask(originalInfo).GetAwaiter().GetResult();
					// since the fetched result could be completely different, non-greedy merging is important
					originalInfo = merger.Merge(originalInfo, parsedInfo, greedy: false);
				}

				originalInfo.WriteToFile(source.FileInfo.FullName); // write new metadata back
				return source;
			}));

			// Move the finished files into the downloads folder
			pipelineBuilder.Append(new SourceMover("/downloads", keepName: true));
			
			scheduler.Add(pipelineBuilder);
			
			scheduler.Start();

			scheduler.Join();
		}

		private class TelegramScheduler : TelegramBotBaseScheduler
		{
			private static readonly ILog Log = LogManager.GetLogger(typeof(TelegramScheduler));

			private readonly ConcurrentDictionary<IFileSource, Chat> _chatMapping =
				new ConcurrentDictionary<IFileSource, Chat>();

			public TelegramScheduler(IExecutor executor, string apiToken, UserList userList = UserList.None,
				IWebProxy proxy = null) :
				base(executor, apiToken, userList, proxy)
			{
				WelcomeText = "Hey there! Just send me youtube links ... :)";
			}

			protected override void ExecuteStart()
			{
				base.ExecuteStart();

				LifeCycleEvent += OnSchedulerEvent;
				Executor.LifeCycleEvent += OnExecutorEvent;
			}

			protected override void ExecuteStop()
			{
				base.ExecuteStop();

				LifeCycleEvent -= OnSchedulerEvent;
				Executor.LifeCycleEvent -= OnExecutorEvent;
			}

			private void OnExecutorEvent(object sender, ExecutorLifeCycleEventArgs args)
			{
				if (args.EventType == ExecutorLifeCycleEventType.SourceExecutionFinished)
				{
					if (_chatMapping.TryRemove(args.FileSource, out var chat))
					{
						Finish(chat.Id, args.FileSource);
					}
				}
				else if (args.EventType == ExecutorLifeCycleEventType.SourceExecutionFailed)
				{
					if (_chatMapping.TryRemove(args.FileSource, out var chat))
					{
						SendTextMessage(chat.Id, "There was an error with your request.\nDid you provide a valid URL?");
						SendTextMessage(chat.Id, args.Error.Message); // you shouldn't send exceptions to any client
					}
				}
			}

			private void OnSchedulerEvent(object sender, SchedulerLifeCycleEventArgs args)
			{
				if (args.EventType == SchedulerLifeCycleEventType.SourceDiscarded)
				{
					if (_chatMapping.TryRemove(args.Source, out var chat))
					{
						SendTextMessage(chat.Id,
							$"Your request for the file {args.Source?.FileName} has been discarded.");
					}
				}
			}

			private void Finish(long chatId, IFileSource source)
			{
				var f = TagLib.File.Create(source.FileInfo.FullName, ReadStyle.Average);

				SendTextMessage(chatId,
					$"{source.FileName}\nTitle:\t{f.Tag.Title}\nArtists:\t{string.Join(", ", f.Tag.Performers)}");

				BotClient.SendAudioAsync(
					chatId: chatId,
					audio: File.OpenRead(source.FileInfo.FullName),
					caption: f.Tag.Lyrics,
					duration: (int) f.Properties.Duration.TotalSeconds,
					performer: string.Join(' ', f.Tag.Performers),
					title: f.Tag.Title
				);
			}

			protected override void ProcessTextMessage(Message message)
			{
				try
				{
					SendTextMessage(message, "brb ...");

					Log.Info($"{message.From.Username} + {message.From.FirstName} requested {message.Text}");
					var youtubeSource = new YoutubeDlSource(message.Text, Enums.AudioFormat.mp3, true);
					IFileSource source = new FileSource(youtubeSource,
						Path.Combine(Path.GetTempPath(), "TicTacTube"));

					if (!_chatMapping.TryAdd(source, message.Chat))
					{
						throw new Exception("File source could not be added.");
					}

					Execute(source);

					Log.Info($"{message.From.Username} + {message.From.FirstName} downloading {source.FileName}");
				}
				catch (Exception e)
				{
					SendTextMessage(message, e.Message);
					Log.Error(e);
				}
			}
		}
	}
}