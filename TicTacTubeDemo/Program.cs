using System;
using System.IO;
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
using TicTacTubeCore.Soundcloud.Processors.Media.Songs;
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

			var scheduler = new TelegramScheduler(File.ReadAllText("telegram.token"));
			var pipelineBuilder = new DataPipelineBuilder();

			var fetcher = new GeniusSongInfoFetcher(File.ReadAllText("genius.token"));
			var extractor = new SongInfoExtractor();

			pipelineBuilder.Append(new LambdaProcessor(source =>
			{
				var info = extractor.ExtractFromString(source.FileName);

				info.WriteToFile(source.FileInfo.FullName);
				info = fetcher.ExtractAsyncTask(info).GetAwaiter().GetResult();
				info.WriteToFile(source.FileInfo.FullName);

				return source;
			}));
			scheduler.Add(pipelineBuilder);

			scheduler.Start();

			var soundcloud = new SoundcloudSongInfoFetcher();
			var soundcloudInfo = soundcloud.ExtractFromStringAsyncTask("https://soundcloud.com/roger-slato/dreamer-remode").GetAwaiter().GetResult();

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

						Log.Info($"{message.From.Username} + {message.From.FirstName} downloaded {source.FileName}");

						var f = TagLib.File.Create(source.FileInfo.FullName, ReadStyle.Average);

						SendTextMessage(message,
							$"{source.FileName}\nTitle:\t{f.Tag.Title}\nArtists:\t{string.Join(", ", f.Tag.Performers)}");


						BotClient.SendAudioAsync(message.Chat.Id,
							new FileToSend(source.FileInfo.FullName, File.OpenRead(source.FileInfo.FullName)), f.Tag.Lyrics,
							(int) f.Properties.Duration.TotalSeconds, string.Join(' ', f.Tag.Performers), f.Tag.Title);
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