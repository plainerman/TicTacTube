using System;
using System.IO;
using log4net;
using NYoutubeDL.Helpers;
using TicTacTubeCore.Sources.Files.External;
using TicTacTubeCore.YoutubeDL.Utils.Extensions;

namespace TicTacTubeCore.YoutubeDL.Sources.Files.External
{
	/// <summary>
	/// An external source that is acquired by the program youtube-dl (see <a href="https://youtube-dl.org/">Youtube-DL</a>).
	/// 
	/// For it to work properly, it is based if you have the following programs in your Path variable:
	/// <list type="bullet">
	///		<item>
	///			<description>Youtube-DL</description>
	///		</item>
	///		<item>
	///			<description>FFMPEG or AVCONV</description>
	///		</item>
	///		<item>
	///			<description>PhantomJS</description>
	///		</item>
	/// </list>
	/// 
	/// On linux they probably install all together.
	/// </summary>
	public class YoutubeDlSource : UrlSource
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(YoutubeDlSource));

		/// <summary>
		/// The Youtube-Download object that can be used to define the download behaviour.
		/// </summary>
		public NYoutubeDL.YoutubeDL YoutubeDl { get; }

		/// <summary>
		/// Create a new Youtube-DL source that downloads videos with a given video and audio format.
		/// </summary>
		/// <param name="url">The url. Here is a list of the <a href="https://rg3.github.io/youtube-dl/supportedsites.html">supported sites</a>.</param>
		/// <param name="videoFormat">The video format the video will be downloaded.</param>
		/// <param name="audioFormat">The audio format that is used internally.</param>
		/// <param name="lazyLoading">If <c>true</c>, the file will be fetched as late as possible.</param>
		public YoutubeDlSource(string url, Enums.VideoFormat videoFormat = Enums.VideoFormat.best, Enums.AudioFormat audioFormat = Enums.AudioFormat.best, bool lazyLoading = false) : base(url, lazyLoading)
		{
			YoutubeDl = new NYoutubeDL.YoutubeDL { VideoUrl = url };
			YoutubeDl.Options.VideoFormatOptions.Format = videoFormat;
			YoutubeDl.Options.PostProcessingOptions.AudioFormat = audioFormat;
			YoutubeDl.Options.DownloadOptions.ExternalDownloader = Enums.ExternalDownloader.aria2c;
		}

		/// <summary>
		/// Create a new Youtube-DL source that downloads audio with a given audio format.
		/// </summary>
		/// <param name="url">The url. Here is a list of the <a href="https://rg3.github.io/youtube-dl/supportedsites.html">supported sites</a>.</param>
		/// <param name="audioFormat">The audio format the file will be stored. Only specific audio formats are supported (i.e. not <see cref="Enums.AudioFormat.best"/>).</param>
		/// <param name="lazyLoading">If <c>true</c>, the file will be fetched as late as possible.</param>
		public YoutubeDlSource(string url, Enums.AudioFormat audioFormat, bool lazyLoading = false) : this(url, Enums.VideoFormat.best, audioFormat, lazyLoading)
		{
			if (audioFormat == Enums.AudioFormat.best)
			{
				throw new InvalidOperationException($"{audioFormat} not support—please specify it specifically.");
			}

			YoutubeDl.Options.PostProcessingOptions.ExtractAudio = true;
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

		/// <summary>
		/// Set the finished path (i.e. the path where the downloaded file will be stored). 
		/// </summary>
		protected virtual void SetFinishedPath()
		{
			YoutubeDl.StandardOutputEvent += SetFinishedPath;
			// this works only for video files, if we have an audio, we have to find the output file ourselves ...
			var process = YoutubeDl.Download(true);
			
			process.WaitForExit();
			if (YoutubeDl.Options.PostProcessingOptions.ExtractAudio)
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