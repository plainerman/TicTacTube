using System;
using log4net;
using log4net.Config;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using NYoutubeDL.Helpers;
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
		private class TelegramScheduler : TelegramBotBaseScheduler
		{
			private static readonly ILog Log = LogManager.GetLogger(typeof(TelegramScheduler));

			public TelegramScheduler(string apiToken, UserList userList = UserList.None, IWebProxy proxy = null) : base(apiToken, userList, proxy)
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

						var f = TagLib.File.Create(source.FileInfo.FullName, TagLib.ReadStyle.Average);

						SendTextMessage(message, $"{source.FileName}\nTitle:\t{f.Tag.Title}\nArtists:\t{string.Join(", ", f.Tag.Performers)}");


						BotClient.SendAudioAsync(message.Chat.Id,
							new FileToSend(source.FileInfo.FullName, File.OpenRead(source.FileInfo.FullName)), f.Tag.Lyrics,
							(int)f.Properties.Duration.TotalSeconds, string.Join(' ', f.Tag.Performers), f.Tag.Title);
					}
					catch (Exception e)
					{
						SendTextMessage(message, e.Message);
						Log.Error(e);
					}
				});
			}
		}
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
			scheduler.Join();

			//var scheduler = new EventFiringScheduler();
			//var pipelineBuilder = new DataPipelineBuilder();

			////new MediaRenamer<SongInfo>("This is my pattern {Title} {Artists} {Title}", new SongInfoExtractor());

			////string[] testVals = { "WE ARE FURY - Waiting (feat. Olivia Lunny)",
			////	"Laura Brehm - Breathe (Last Heroes & Crystal Skies Remix) (Lyric Video)",
			////	"Laura Brehm - Breathe (Last Heroes & Crystal Skies Remix) (Lyric Video)",
			////	"Rita Ora - Your Song(Official Lyric Video)",
			////	"Rita Ora - Your Song(Official Video)",
			////	"Dua Lipa - New Rules(Official Music Video)",
			////	"Snugs - Radio Silence (ft. HAILZ) [Lyric Video]",
			////	"Selena Gomez, Marshmello - Wolves"
			////};

			//	/*var extractor = new SongInfoExtractor();
			//foreach (var testVal in testVals)
			//{
			//	extractor.ExtractFromFileName(testVal);
			//}*/


			////var source = new FileSource(@"C:\Marshmello - You And Me (Official Music Video).mp3");
			///*var source =
			//	new FileSource(
			//		new UrlSource(@"https://www.dropbox.com/s/4uz4sx5q3mrfg4s/Old%20Telephone%20Uncompressed%20WAVE.wav?dl=1"),
			//		@"C:\Users\plain\Desktop\newFolder\subfolder\");
			//*/

			//FileSource source = @"C:\Users\plain\Desktop\test.mp3";

			//pipelineBuilder
			//	.Append(new SourceCloner(@"C:\Users\plain\Desktop\newFolder", false, true))
			//	.Append(new SourceMover(@"C:\Users\plain\Desktop\newFolder\test2.mp3"));

			////pipelineBuilder
			////	.Append(new SourceConverter(Type.Mp3))
			////	.Append(new MetaDataFiller())
			////	.Append(new MusicDetector())
			////	.Append(new MusicRenamer())
			////	.Append(new ConditionalProcessor(file => file.Size > 10000),
			////		new SourceMover(destination: "/home/plainer/podcasts/"),
			////		new SourceMover(destination: "/home/plainer/music/"))
			////	.Split(new SourceDuplicator("(2)"), new MultiProcessor(new SymlinkCreator()..., ... /*nested pipeline */), new SourceMover("/home/src"));


			//scheduler.Add(pipelineBuilder);
			//scheduler.Start();
			//scheduler.Fire(source);

			//Console.ReadKey();
		}
	}
}