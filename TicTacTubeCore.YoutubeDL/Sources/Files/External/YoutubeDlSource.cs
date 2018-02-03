using System;
using System.IO;
using log4net;
using NYoutubeDL.Helpers;
using TicTacTubeCore.Sources.Files.External;
using TicTacTubeCore.YoutubeDL.Utils.Extensions;

namespace TicTacTubeCore.YoutubeDL.Sources.Files.External
{
	public class YoutubeDlSource : UrlSource
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(YoutubeDlSource));

		protected NYoutubeDL.YoutubeDL YoutubeDl { get; }
		protected bool Audio;

		public YoutubeDlSource(string url, Enums.VideoFormat videoFormat = Enums.VideoFormat.best, Enums.AudioFormat audioFormat = Enums.AudioFormat.best, bool lazyLoading = false) : base(url, lazyLoading)
		{
			YoutubeDl = new NYoutubeDL.YoutubeDL { VideoUrl = url };

			YoutubeDl.Options.VideoFormatOptions.Format = videoFormat;
			YoutubeDl.Options.PostProcessingOptions.AudioFormat = audioFormat;
			YoutubeDl.Options.DownloadOptions.ExternalDownloader = Enums.ExternalDownloader.aria2c;
		}

		public YoutubeDlSource(string url, Enums.AudioFormat format, bool lazyLoading = false) : this(url, Enums.VideoFormat.best, format, lazyLoading)
		{
			if (format == Enums.AudioFormat.best)
			{
				throw new InvalidOperationException($"{format} not support—please specify it specifically.");
			}

			YoutubeDl.Options.PostProcessingOptions.ExtractAudio = true;
			Audio = true;
		}

		/// <inheritdoc />
		protected override void Download(string destinationPath)
		{
			DownloadAsync(destinationPath);
			CurrentDownloadTask.Wait();
			CurrentDownloadTask = null;
		}

		/// <inheritdoc />
		protected override void DownloadAsync(string destinationPath)
		{
			YoutubeDl.Options.FilesystemOptions.Output = Path.Combine(destinationPath, "%(title)s.%(ext)s");
			YoutubeDl.Options.VerbositySimulationOptions.GetFilename = true;

			SetFinishedPath();

			YoutubeDl.StandardErrorEvent += PrintError;

			CurrentDownloadTask = TaskUtils.RunProcessInTask(YoutubeDl.Download(true)).ContinueWith(prevTask =>
			{
				YoutubeDl.StandardErrorEvent -= PrintError;
			});

			void PrintError(object sender, string output) => Log.Error(output);
		}

		protected virtual void SetFinishedPath()
		{
			YoutubeDl.StandardOutputEvent += SetFinishedPath;
			// this works only for video files, if we have an audio, we have to find the output file ourselves ...
			var process = YoutubeDl.Download(true);
			
			process.WaitForExit();
			if (Audio)
			{
				var audioFormat = YoutubeDl.Options.PostProcessingOptions.AudioFormat;
				string extension = audioFormat.ToString();

				if (audioFormat == Enums.AudioFormat.threegp)
				{
					extension = "3gp";
				}
				else if (audioFormat == Enums.AudioFormat.vorbis)
				{
					extension = "ogg";
				}

				FinishedPath = Path.Combine(Path.GetDirectoryName(FinishedPath),
					$"{Path.GetFileNameWithoutExtension(FinishedPath)}.{extension}");
			}

			YoutubeDl.Options.VerbositySimulationOptions.GetFilename = false;

			YoutubeDl.StandardOutputEvent -= SetFinishedPath;

			void SetFinishedPath(object sender, string output)
			{
				FinishedPath = output.Trim();
			}
		}
	}
}