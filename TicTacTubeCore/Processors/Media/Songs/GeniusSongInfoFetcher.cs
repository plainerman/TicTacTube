using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Genius;
using Genius.Models;
using Newtonsoft.Json.Linq;
using TagLib;
using TicTacTubeCore.Sources.Files;
using File = System.IO.File;

namespace TicTacTubeCore.Processors.Media.Songs
{
	/// <summary>
	///     A song info extractor that queries the already stored info.
	/// </summary>
	public class GeniusSongInfoFetcher : IMediaInfoExtractor<SongInfo>
	{
		/// <summary>
		///     The genius client that is used to perform queris on Genius.com.
		/// </summary>
		protected readonly GeniusClient GeniusClient;

		protected readonly bool OverrideData;

		public GeniusSongInfoFetcher(string geniusApiKey, bool overrideData)
		{
			if (string.IsNullOrWhiteSpace(geniusApiKey))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(geniusApiKey));
			OverrideData = overrideData;

			GeniusClient = new GeniusClient(geniusApiKey);
		}

		public async Task<SongInfo> ExtractAsyncTask(IFileSource source) => await ExtractAsyncTask(SongInfo.ReadFromFile(source.FileInfo.FullName));

		public async Task<SongInfo> ExtractAsyncTask(SongInfo currentInfo)
		{
			return await Task.Run(async () =>
			{
				string searchTerm = currentInfo.Title;

				if (currentInfo.Artists.Length > 0)
					searchTerm = currentInfo.Artists[0] + searchTerm;

				var result = (await GeniusClient.SearchClient.Search(TextFormat.Dom, searchTerm)).Response;

				if (result.Count > 0)
				{
					// TODO: dont get the first one, get the first one with the type song
					var bestHit = (JObject)result[0].Result;

					long songId = bestHit.GetValue("id").Value<long>();

					var song = await GeniusClient.SongsClient.GetSong(TextFormat.Html, songId.ToString());
					currentInfo = await SetSong(currentInfo, song.Response);
				}

				return currentInfo;
			});
		}

		protected virtual async Task<SongInfo> SetSong(SongInfo info, Song song)
		{
			string coverArtDestination = Path.GetTempFileName();

			var webClient = new WebClient();
			var downloadCoverArt = webClient.DownloadFileTaskAsync(song.HeaderImageUrl, coverArtDestination);

			if (!string.IsNullOrWhiteSpace(song.ReleaseDate))
			{
				var releaseDate = DateTime.ParseExact(song.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				info.Year = (uint)releaseDate.Year;
			}

			if (song.Album != null)
			{
				info.Album = song.Album?.Name;
				info.AlbumArtists = new[] { song.Album.Artist.Name };
			}

			var artists = new List<string>();

			if (OverrideData)
			{
				info.Title = song.Title;

				artists.Add(song.PrimaryArtist.Name);
				artists.AddRange(song.FeaturedArtists.Select(artist => artist.Name));
			}
			else
			{
				artists.AddRange(info.Artists);

				if (artists.Count <= 0)
					artists.Add(song.PrimaryArtist.Name);

				foreach (var artist in song.FeaturedArtists)
				{
					if (!artists.Contains(artist.Name))
						artists.Add(artist.Name);
				}
			}

			info.Artists = artists.ToArray();

			// Set the cover art and delete the file afterwards
			await downloadCoverArt;

			// TODO: keep old images?
			info.Pictures = new IPicture[] { new Picture(coverArtDestination) };
			File.Delete(coverArtDestination);
			webClient.Dispose();

			return info;
		}
	}
}