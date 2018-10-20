using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using NYoutubeDL.Helpers;
using NYoutubeDL.Models;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Sources.Files.External;
using TicTacTubeCore.YoutubeDL.Utils.Extensions;

namespace TicTacTubeCore.YoutubeDL.Sources.Files.External
{
	//TODO: add documention
	//TODO: move to own class
	public interface IYoutubeDlSource
	{
		/// <summary>
		/// The title of either the playlist or the single video. This may be <c>null</c> until fully fetched.
		/// </summary>
		string Title { get; }

		/// <summary>
		/// If the specified youtube-dl source is a playlist, multiple child sources will be created. The source itself will point to the folder.
		/// This may be <c>null</c> until fully fetched.
		/// </summary>
		IFileSource[] ChildSources { get; }

		/// <summary>
		/// Determines whether the object is a playlist. If this source is a playlist, the source will point to a folder instead of a file. 
		/// </summary>
		bool IsPlaylist { get; }
	}

	/// <summary>
	///     An external source that is acquired by the program youtube-dl
	///		(see <a href="https://youtube-dl.org/">Youtube-DL</a>).
	///     For it to work properly, it is based if you have the following programs in your Path variable:
	///     <list type="bullet">
	///         <item>
	///             <description>Youtube-DL</description>
	///         </item>
	///         <item>
	///             <description>FFMPEG or AVCONV</description>
	///         </item>
	///         <item>
	///             <description>PhantomJS</description>
	///         </item>
	///     </list>
	///     On linux they probably install all together.
	/// 
	///		If the specified URL consists of multiple elements (e.g. a playlist), this source will point to a folder where all sources are contained.
	/// </summary>
	public class YoutubeDlSource : UrlSource, IYoutubeDlSource
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(YoutubeDlSource));

		/// <summary>
		///     The Youtube-Download object that can be used to define the download behaviour.
		/// </summary>
		public NYoutubeDL.YoutubeDL YoutubeDl { get; }

		/// <inheritdoc />
		public string Title { get; protected set; }

		/// <inheritdoc />
		public IFileSource[] ChildSources { get; protected set; }

		/// <inheritdoc />
		public bool IsPlaylist { get; protected set; }

		/// <summary>
		///     Create a new Youtube-DL source that downloads videos with a given video and audio format.
		/// </summary>
		/// <param name="url">
		///     The url. Here is a list of the
		///     <a href="https://rg3.github.io/youtube-dl/supportedsites.html">supported sites</a>.
		/// </param>
		/// <param name="videoFormat">The video format the video will be downloaded.</param>
		/// <param name="audioFormat">The audio format that is used internally.</param>
		/// <param name="lazyLoading">If <c>true</c>, the file will be fetched as late as possible.</param>
		public YoutubeDlSource(string url, Enums.VideoFormat videoFormat = Enums.VideoFormat.best,
			Enums.AudioFormat audioFormat = Enums.AudioFormat.best, bool lazyLoading = false) : base(url, lazyLoading)
		{
			YoutubeDl = new NYoutubeDL.YoutubeDL { VideoUrl = url };
			YoutubeDl.Options.VideoFormatOptions.Format = videoFormat;
			YoutubeDl.Options.PostProcessingOptions.AudioFormat = audioFormat;
			YoutubeDl.Options.DownloadOptions.ExternalDownloader = Enums.ExternalDownloader.aria2c;
			YoutubeDl.Options.FilesystemOptions.RestrictFilenames = true;
		}

		/// <summary>
		///     Create a new Youtube-DL source that downloads audio with a given audio format.
		/// </summary>
		/// <param name="url">
		///     The url. Here is a list of the
		///     <a href="https://rg3.github.io/youtube-dl/supportedsites.html">supported sites</a>.
		/// </param>
		/// <param name="audioFormat">
		///     The audio format the file will be stored. Only specific audio formats are supported (i.e. not
		///     <see cref="Enums.AudioFormat.best" />).
		/// </param>
		/// <param name="lazyLoading">If <c>true</c>, the file will be fetched as late as possible.</param>
		public YoutubeDlSource(string url, Enums.AudioFormat audioFormat, bool lazyLoading = false) : this(url,
			Enums.VideoFormat.best, audioFormat, lazyLoading)
		{
			if (audioFormat == Enums.AudioFormat.best)
				throw new InvalidOperationException($"{audioFormat} not supported—please specify it specifically.");

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
			YoutubeDl.Options.VerbositySimulationOptions.GetFilename = true;

			var info = YoutubeDl.GetDownloadInfo();

			IsPlaylist = info is PlaylistDownloadInfo;
			Title = info.Title;

			if (IsPlaylist)
			{
				Log.InfoFormat("Downloading playlist {0}", Title);

				destinationPath = AddUniqueFolder(destinationPath, Title);

				if (!Directory.Exists(destinationPath))
				{
					Log.InfoFormat("Creating directory {0}", destinationPath);
					Directory.CreateDirectory(destinationPath);
				}

				//TODO make this an option
				YoutubeDl.Options.GeneralOptions.AbortOnError = false;
			}

			YoutubeDl.Options.FilesystemOptions.Output = Path.Combine(destinationPath, "%(title)s.%(ext)s");

			SetFinishedPath(info);

			YoutubeDl.StandardErrorEvent += PrintError;

			CurrentDownloadTask = TaskUtils.RunProcessInTask(YoutubeDl.Download(true))
				.ContinueWith(prevTask => { YoutubeDl.StandardErrorEvent -= PrintError; });

			void PrintError(object sender, string output) => Log.Error(output);
		}

		/// <summary>
		///     Set the finished path (i.e. the path where the downloaded file will be stored).
		/// </summary>
		/// <param name="info">The download info from the current download process.</param>
		protected virtual void SetFinishedPath(DownloadInfo info)
		{
			var downloadedPaths = new List<string>();

			YoutubeDl.StandardOutputEvent += SetFinishedPath;
			// this works only for video files, if we have an audio, we have to find the output file ourselves ...
			var process = YoutubeDl.Download(true);

			process.WaitForExit();
			YoutubeDl.StandardOutputEvent -= SetFinishedPath;

			// fix the path for all (or a single) audio file source
			if (YoutubeDl.Options.PostProcessingOptions.ExtractAudio)
			{
				downloadedPaths = downloadedPaths.Select(path =>
				{
					string extension = AudioFormatToExtension(YoutubeDl.Options.PostProcessingOptions.AudioFormat);

					return Path.Combine(Path.GetDirectoryName(path),
						$"{Path.GetFileNameWithoutExtension(path)}.{extension}");
				}).ToList();
			}

			// Create the child sources
			if (IsPlaylist)
			{
				var videosInPlaylist = ((PlaylistDownloadInfo)info).Videos;
				ChildSources = new IFileSource[downloadedPaths.Count];

				for (int i = 0; i < downloadedPaths.Count; i++)
				{
					Log.DebugFormat("Downloading {0}...", videosInPlaylist[i].Title);
					ChildSources[i] = new FileSource(new DummyYoutubeDlSource(videosInPlaylist[i].Url, videosInPlaylist[i].Title), downloadedPaths[i], true);
				}
			}

			FinishedPath = IsPlaylist ? Path.GetDirectoryName(downloadedPaths[0]) : downloadedPaths[0];

			YoutubeDl.Options.VerbositySimulationOptions.GetFilename = false;

			void SetFinishedPath(object sender, string output)
			{
				// TODO: utf8 in video title (https://www.youtube.com/watch?v=Odum0r9gxA8) or playlist title!
				// TODO: space somewhere in download folder causes problems with youtube-dl setdownloadpath
				// ReSharper disable once AccessToModifiedClosure
				// This is fixed by removing the event after exit
				downloadedPaths.Add(output.Trim());
			}
		}

		/// <summary>
		/// This method converts a given audioformat to the corresponding file extension.
		/// </summary>
		/// <param name="audioFormat">The audioformat that will be checked.</param>
		/// <returns>The default 3-letter file extension of the audio format.</returns>
		protected static string AudioFormatToExtension(Enums.AudioFormat audioFormat)
		{
			if (audioFormat == Enums.AudioFormat.threegp)
				return "3gp";
			if (audioFormat == Enums.AudioFormat.vorbis)
				return "ogg";

			return audioFormat.ToString();
		}

		//TODO: make available to other classes
		//TODO: documentation
		protected static string CleanFileName(string fileName)
		{
			// TODO: removing the space is currently a hack
			return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty)).Replace(" ", string.Empty);
		}

		//TODO: make available to other classes
		//TODO: documentation
		protected static string AddUniqueFolder(string baseFolder, string newFolder)
		{
			newFolder = CleanFileName(newFolder);
			string newFolderName = newFolder;

			string returnedFolder;
			int i = 0;
			while (Directory.Exists(returnedFolder = Path.Combine(baseFolder, newFolderName)))
			{
				newFolderName = $"{newFolder}-{++i}";
			}

			return returnedFolder;
		}

		//TODO: think about where to put the class (should it really be a subclass?)
		//TODO: add documentation
		//TODO: rename to childdlsource or something similar
		protected class DummyYoutubeDlSource : UrlSource, IYoutubeDlSource
		{
			public DummyYoutubeDlSource(string url, string title) : base(url, true)
			{
				Title = title;
			}

			protected override void Download(string destinationPath)
			{ }

			protected override void DownloadAsync(string destinationPath)
			{ }

			public string Title { get; }
			public IFileSource[] ChildSources => null;
			public bool IsPlaylist => false;
		}
	}
}