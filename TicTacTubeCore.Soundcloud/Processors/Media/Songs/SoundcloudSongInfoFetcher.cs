using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using TagLib;
using TicTacTubeCore.Processors.Media;
using TicTacTubeCore.Processors.Media.Songs;
using TicTacTubeCore.Sources.Files;
using File = System.IO.File;

namespace TicTacTubeCore.Soundcloud.Processors.Media.Songs
{
	public class SoundcloudSongInfoFetcher : IMediaInfoExtractor<SongInfo>
	{
		public string UserAgent { get; set; } = "Mozilla/5.0 (X11; Linux x86_64; rv:10.0) Gecko/20100101 Firefox/10.0";
		public SongInfoExtractor SongInfoExtractor { get; }

		public SoundcloudSongInfoFetcher()
		{
			SongInfoExtractor = new SongInfoExtractor();
		}

		public Task<SongInfo> ExtractAsyncTask(IFileSource source)
		{
			throw new System.NotImplementedException();
		}

		public async Task<SongInfo> ExtractAsyncTask(string url)
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

				var info = SongInfoExtractor.ExtractFromString(HttpUtility.HtmlDecode(coverArtNode.Attributes["alt"].Value));

				var genreNode = articleNode.SelectSingleNode("//header/meta[@itemprop='genre']");
				if (genreNode == null)
				{
					throw new MissingFieldException("No genre node found.");
				}

				info.Genres = HttpUtility.HtmlDecode(genreNode.Attributes["content"].Value).Split('&');

				await downloadCoverArt;
				info.Pictures = new[] { SongInfo.CreatePictureFrame(coverArtDestinationFile, PictureType.FrontCover) };
				File.Delete(coverArtDestinationFile);

				return info;
			}
		}

		private async Task<HtmlDocument> CreateDoc(WebClient webClient, string url)
		{
			webClient.Headers.Add("user-agent", UserAgent);
			string pageContent = await webClient.DownloadStringTaskAsync(new Uri(url));

			var doc = new HtmlDocument();
			doc.LoadHtml(pageContent);

			return doc;
		}

		private class DecompressingWebClient : WebClient
		{
			protected override WebRequest GetWebRequest(Uri address)
			{
				var request = base.GetWebRequest(address) as HttpWebRequest;
				request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
				return request;
			}
		}
	}
}