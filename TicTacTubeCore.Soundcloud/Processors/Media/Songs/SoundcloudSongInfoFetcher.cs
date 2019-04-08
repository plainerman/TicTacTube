using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using TagLib;
using TicTacTubeCore.Processors.Media;
using TicTacTubeCore.Processors.Media.Songs;
using TicTacTubeCore.Soundcloud.Processors.Media.Songs.Exceptions;
using TicTacTubeCore.Soundcloud.Utils.Web;
using File = System.IO.File;

namespace TicTacTubeCore.Soundcloud.Processors.Media.Songs
{
	/// <summary>
	///     A songinfo fetcher that fetches data from a soundcloud url.
	///     Currently, this fetcher simulates a web browser, which might be against soundcloud terms of service.
	///     Only use it with soundclouds written permission.
	///     This is currently only a workaround for testing, as soundclouds API-validation is currently disabled.
	/// </summary>
	public class SoundcloudSongInfoFetcher : IMediaTextInfoExtractor<SongInfo>
	{
		/// <summary>
		///     The schema string for a music recording.
		///     This is used to test, whether the page actually is a record.
		/// </summary>
		protected const string MusicRecordingSchema = @"http://schema.org/MusicRecording";

		/// <summary>
		///     The user agent that will be sent to soundcloud.
		/// </summary>
		public string UserAgent { get; set; } = "Mozilla/5.0 (X11; Linux x86_64; rv:10.0) Gecko/20100101 Firefox/10.0";

		/// <summary>
		///     The extractor that is used to parse the title of the soundcloud song.
		/// </summary>
		public IMediaTextInfoExtractor<SongInfo> SongInfoExtractor { get; }

		/// <summary>
		///     Create a new soundcloud song fetcher with a default <see cref="SongInfoExtractor" />.
		/// </summary>
		public SoundcloudSongInfoFetcher()
		{
			SongInfoExtractor = new SongInfoExtractor(false);
		}

		private async Task<HtmlDocument> CreateDoc(WebClient webClient, string url)
		{
			webClient.Headers.Add("user-agent", UserAgent);
			string pageContent = await webClient.DownloadStringTaskAsync(new Uri(url));

			var doc = new HtmlDocument();
			doc.LoadHtml(pageContent);

			return doc;
		}

		/// <summary>
		///     This method extracts a <see cref="SongInfo"/> from a given string (<paramref name="url" />), which is a soundcloud url.
		/// </summary>
		/// <param name="url">
		///     The url to the Soundcloud song. May not be <code>null</code>.
		/// </param>
		/// <returns>A <see cref="SongInfo" /> containing the extracted information.</returns>
		public async Task<SongInfo> ExtractFromStringAsyncTask(string url)
		{
			if (url == null) throw new ArgumentNullException(nameof(url));

			using (var webClient = new DecompressingWebClient())
			{
				var doc = await CreateDoc(webClient, url);

				var articleNode = doc.DocumentNode.SelectSingleNode("//article[@itemscope]");
				if (articleNode == null || articleNode.Attributes["itemtype"].Value != MusicRecordingSchema)
					throw new InvalidSoundcloudPageTypeException("Page type not supported.");

				var coverArtNode = articleNode.SelectSingleNode("//p/img");

				if (coverArtNode == null) throw new MissingCoverArtException("No cover art node found.");

				// begin with downloading the cover art (since it is async)
				string coverArtUrl = coverArtNode.Attributes["src"].Value;

				string coverArtDestinationFile = Path.GetTempFileName();
				var downloadCoverArt = webClient.DownloadFileTaskAsync(new Uri(coverArtUrl), coverArtDestinationFile);

				var infoTask =
					SongInfoExtractor.ExtractFromStringAsyncTask(
						HttpUtility.HtmlDecode(coverArtNode.Attributes["alt"].Value));

				var genreNode = articleNode.SelectSingleNode("//header/meta[@itemprop='genre']");
				if (genreNode == null) throw new MissingGenreException("No genre node found.");

				var info = await infoTask;

				info.Genres = HttpUtility.HtmlDecode(genreNode.Attributes["content"].Value)
					?.Split('&').Select(g => g.Trim())
					.ToArray();

				await downloadCoverArt;
				info.Pictures = new[] { SongInfo.CreatePictureFrame(coverArtDestinationFile, PictureType.FrontCover) };
				File.Delete(coverArtDestinationFile);

				return info;
			}
		}
	}
}