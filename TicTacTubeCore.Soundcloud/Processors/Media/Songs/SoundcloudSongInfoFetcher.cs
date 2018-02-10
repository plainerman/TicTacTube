using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using TagLib;
using TicTacTubeCore.Processors.Media;
using TicTacTubeCore.Processors.Media.Songs;
using File = System.IO.File;

namespace TicTacTubeCore.Soundcloud.Processors.Media.Songs
{
	/// <summary>
	/// A songinfo fetcher that fetches data from a soundcloud url.
	/// Currently, this fetcher simulates a web browser, which might be against soundcloud terms of service.
	/// Only use it with soundclouds written permission (as soundclouds API-validation is currently disabled).
	/// </summary>
	public class SoundcloudSongInfoFetcher : IMediaTextInfoExtractor<SongInfo>
	{
		/// <summary>
		/// The user agent that will be sent to soundcloud.
		/// </summary>
		public string UserAgent { get; set; } = "Mozilla/5.0 (X11; Linux x86_64; rv:10.0) Gecko/20100101 Firefox/10.0";
		/// <summary>
		/// The extractor that is used to parse the title of the soundcloud song.
		/// </summary>
		public IMediaTextInfoExtractor<SongInfo> SongInfoExtractor { get; }

		/// <summary>
		/// Create a new soundcloud song fetcher with a default <see cref="SongInfoExtractor"/>.
		/// </summary>
		public SoundcloudSongInfoFetcher()
		{
			SongInfoExtractor = new SongInfoExtractor();
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
		///     This method extracts a songinfo from a given string (<paramref name="url" />), which is a soundcloud url.
		/// </summary>
		/// <param name="url">
		///     The url to the soundcloud song.
		/// </param>
		/// <returns>A <see cref="SongInfo" /> containing the extracted information.</returns>
		public async Task<SongInfo> ExtractFromStringAsyncTask(string url)
		{
			//TODO: custom exception types
			using (var webClient = new DecompressingWebClient())
			{
				var doc = await CreateDoc(webClient, url);

				var articleNode = doc.DocumentNode.SelectSingleNode("//article[@itemscope]");
				if (articleNode == null || articleNode.Attributes["itemtype"].Value != @"http://schema.org/MusicRecording")
				{
					throw new InvalidOperationException("Page type not supported.");
				}

				var coverArtNode = articleNode.SelectSingleNode("//p/img");

				if (coverArtNode == null)
				{
					throw new MissingFieldException("No cover art node found.");
				}

				// begin with downloading the cover art (since it is async)
				string coverArtUrl = coverArtNode.Attributes["src"].Value;

				string coverArtDestinationFile = Path.GetTempFileName();
				var downloadCoverArt = webClient.DownloadFileTaskAsync(new Uri(coverArtUrl), coverArtDestinationFile);

				var infoTask = SongInfoExtractor.ExtractFromStringAsyncTask(HttpUtility.HtmlDecode(coverArtNode.Attributes["alt"].Value));

				var genreNode = articleNode.SelectSingleNode("//header/meta[@itemprop='genre']");
				if (genreNode == null)
				{
					throw new MissingFieldException("No genre node found.");
				}

				var info = await infoTask;

				info.Genres = HttpUtility.HtmlDecode(genreNode.Attributes["content"].Value).Split('&');

				await downloadCoverArt;
				info.Pictures = new[] { SongInfo.CreatePictureFrame(coverArtDestinationFile, PictureType.FrontCover) };
				File.Delete(coverArtDestinationFile);

				return info;
			}
		}

		private class DecompressingWebClient : WebClient
		{
			protected override WebRequest GetWebRequest(Uri address)
			{
				var request = (HttpWebRequest) base.GetWebRequest(address);
				request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
				return request;
			}
		}
	}
}