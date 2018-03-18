using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using NYoutubeDL.Helpers;
using TagLib;
using Telegram.Bot.Types;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Processors.Logical;
using TicTacTubeCore.Processors.Media.Songs;
using TicTacTubeCore.Sources.Files;
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

			var scheduler = new TelegramScheduler(File.ReadAllText("telegram.token"), UserList.Whitelist);

			File.ReadLines("white.list").ToList().ForEach(user => scheduler.AddUser(int.Parse(user)));

			var pipelineBuilder = new DataPipelineBuilder();

			var fetcher = new GeniusSongInfoFetcher(File.ReadAllText("genius.token"));
			var extractor = new SongInfoExtractor();

			pipelineBuilder.Append(new LambdaProcessor(source =>
			{
				//TODO: HACK: fix this ugly multiplexing hack
				var multiplexedSources = new List<IFileSource>();
				if (source.ExternalFileSource is IYoutubeDlSource youtubeDlSource && youtubeDlSource.IsPlaylist)
				{
					multiplexedSources.AddRange(youtubeDlSource.ChildSources);
				}
				else
				{
					multiplexedSources.Add(source);
				}

				foreach (var nestedSource in multiplexedSources)
				{
					// TODO: rewrite to nested file
					string songTitle = nestedSource.FileName;

					if (nestedSource.ExternalFileSource is IYoutubeDlSource nestedYoutubeDlSource)
					{
						songTitle = nestedYoutubeDlSource.Title;
					}

					var info = extractor.ExtractFromString(songTitle);

					info.WriteToFile(nestedSource.FileInfo.FullName);
					info = fetcher.ExtractAsyncTask(info).GetAwaiter().GetResult();
					info.WriteToFile(nestedSource.FileInfo.FullName);
				}

				return source;
			}));

			scheduler.Add(pipelineBuilder);

			scheduler.Start();
			scheduler.Join();
		}

		private class TelegramScheduler : TelegramBotBaseScheduler
		{
			private static readonly ILog Log = LogManager.GetLogger(typeof(TelegramScheduler));

			public TelegramScheduler(string apiToken, UserList userList = UserList.None, IWebProxy proxy = null) : base(apiToken,
				userList, proxy)
			{
				WelcomeText = "Hey there! Just send me youtube links ... :)";
			}

			protected override void ProcessTextMessage(Message message)
			{
				Task.Run(() =>
				{
					try
					{
						SendTextMessage(message, "brb ...");

						Log.Info($"{message.From.Username} + {message.From.FirstName} requested {message.Text}");
						var youtubeSource = new YoutubeDlSource(message.Text, Enums.AudioFormat.mp3, true);

						IFileSource source = new FileSource(youtubeSource,
							Path.Combine(Path.GetTempPath(), "TicTacTube"));

						Execute(source);

						//TODO: HACK: fix this ugly multiplexing hack
						var multiplexedSources = new List<IFileSource>();
						if (source.ExternalFileSource is IYoutubeDlSource youtubeDlSource && youtubeDlSource.IsPlaylist)
						{
							multiplexedSources.AddRange(youtubeDlSource.ChildSources);
						}
						else
						{
							multiplexedSources.Add(source);
						}

						foreach (var multiplexedSource in multiplexedSources)
						{
							Log.Info($"{message.From.Username} + {message.From.FirstName} downloaded {multiplexedSource.FileName}");

							var f = TagLib.File.Create(multiplexedSource.FileInfo.FullName, ReadStyle.Average);

							SendTextMessage(message,
								$"{multiplexedSource.FileName}\nTitle:\t{f.Tag.Title}\nArtists:\t{string.Join(", ", f.Tag.Performers)}");


							BotClient.SendAudioAsync(message.Chat.Id,
								new FileToSend(multiplexedSource.FileInfo.FullName, File.OpenRead(multiplexedSource.FileInfo.FullName)), f.Tag.Lyrics,
								(int)f.Properties.Duration.TotalSeconds, string.Join(' ', f.Tag.Performers), f.Tag.Title);
						}
					}
					catch (Exception e)
					{
						SendTextMessage(message, e.Message);
						Log.Error(e);
					}
				});
			}
		}
	}
}