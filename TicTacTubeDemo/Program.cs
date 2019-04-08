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

namespace TicTacTubeDemo
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
			XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

			var scheduler = new TelegramScheduler(new Executor(8, new ExternalSourceComparer()),
				File.ReadAllText("telegram.token"));
			var pipelineBuilder = new DataPipelineBuilder();

			var fetcher = new GeniusSongInfoFetcher();
			var extractor = new SongInfoExtractor(false);

			pipelineBuilder.Append(new LambdaProcessor(source =>
			{
				var info = extractor.ExtractFromString(source.FileName);

				//TODO: SongInfoMerger

				info.WriteToFile(source.FileInfo.FullName);
				info = fetcher.ExtractAsyncTask(info).GetAwaiter().GetResult();
				info.WriteToFile(source.FileInfo.FullName);

				return source;
			}));
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


				BotClient.SendAudioAsync(chatId,
					new FileToSend(source.FileInfo.FullName, File.OpenRead(source.FileInfo.FullName)), f.Tag.Lyrics,
					(int) f.Properties.Duration.TotalSeconds, string.Join(' ', f.Tag.Performers), f.Tag.Title);
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