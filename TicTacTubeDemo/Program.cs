﻿using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Processors.Filesystem;
using TicTacTubeCore.Processors.Media;
using TicTacTubeCore.Processors.Media.Songs;
using TicTacTubeCore.Schedulers;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeDemo
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
			XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

			var scheduler = new EventFiringScheduler();
			var pipelineBuilder = new DataPipelineBuilder();

			new MediaRenamer<SongInfo>("This is my pattern {Title} {Artists} {Title}", new SongInfoExtractor());

			string[] testVals = { "WE ARE FURY - Waiting (feat. Olivia Lunny)",
				"Laura Brehm - Breathe (Last Heroes & Crystal Skies Remix) (Lyric Video)",
				"Laura Brehm - Breathe (Last Heroes & Crystal Skies Remix) (Lyric Video)",
				"Rita Ora - Your Song(Official Lyric Video)",
				"Rita Ora - Your Song(Official Video)",
				"Dua Lipa - New Rules(Official Music Video)",
				"Snugs - Radio Silence (ft. HAILZ) [Lyric Video]",
				"Selena Gomez, Marshmello - Wolves"
			};

		 	/*var extractor = new SongInfoExtractor();
			foreach (var testVal in testVals)
			{
				extractor.ExtractFromFileName(testVal);
			}*/


			//var source = new FileSource(@"C:\Marshmello - You And Me (Official Music Video).mp3");
			/*var source =
				new FileSource(
					new UrlSource(@"https://www.dropbox.com/s/4uz4sx5q3mrfg4s/Old%20Telephone%20Uncompressed%20WAVE.wav?dl=1"),
					@"C:\Users\plain\Desktop\newFolder\subfolder\");
			*/

			FileSource source = @"C:\Users\plain\Desktop\test.mp3";

			pipelineBuilder
				.Append(new SourceCloner(@"C:\Users\plain\Desktop\newFolder", false, true))
				.Append(new SourceMover(@"C:\Users\plain\Desktop\newFolder\test2.mp3"));

			//pipelineBuilder
			//	.Append(new SourceConverter(Type.Mp3))
			//	.Append(new MetaDataFiller())
			//	.Append(new MusicDetector())
			//	.Append(new MusicRenamer())
			//	.Append(new ConditionalProcessor(file => file.Size > 10000),
			//		new SourceMover(destination: "/home/plainer/podcasts/"),
			//		new SourceMover(destination: "/home/plainer/music/"))
			//	.Split(new SourceDuplicator("(2)"), new MultiProcessor(new SymlinkCreator()..., ... /*nested pipeline */), new SourceMover("/home/src"));


			scheduler.Add(pipelineBuilder);
			scheduler.Start();
			scheduler.Fire(source);

			Console.ReadKey();
		}
	}
}