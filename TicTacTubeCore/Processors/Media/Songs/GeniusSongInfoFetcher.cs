using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Genius;
using Genius.Models;
using Newtonsoft.Json.Linq;
using TagLib;
using TagLib.Id3v2;
using TicTacTubeCore.Sources.Files;
using File = System.IO.File;

namespace TicTacTubeCore.Processors.Media.Songs
{
	/// <summary>
	///     A song info extractor that queries the already stored info to https://genius.com and downloads additional data
	///     (e.g. album cover).
	/// </summary>
	public class GeniusSongInfoFetcher : IMediaInfoExtractor<SongInfo>
	{
		/// <summary>
		///     The genius client that is used to perform queris on Genius.com.
		/// </summary>
		protected readonly GeniusClient GeniusClient;

		/// <summary>
		///     Determine whether the title should be overriden (i.e. taken from genius - may result in completely wrong matched
		///     songs) or expanded, fixed, or modified.
		/// </summary>
		public bool OverrideTitle { get; set; } = false;

		/// <summary>
		///     Determine whether the artists should be overriden (i.e. taken from genius - may result in completely wrong matched
		///     songs) or expanded / modified.
		/// </summary>
		public bool OverrideArtists { get; set; }

		/// <summary>
		///     Whether to download the artist image or not. Setting it to <c>true</c> may cause problems when displaying the audio
		///     cover art.
		/// </summary>
		public bool DownloadArtistImage { get; set; } = false;

		/// <summary>
		///     Create a new genius fetcher that requires a <paramref name="geniusApiKey" /> to query on https://genius.com.
		///     Further, <paramref name="overrideArtists" /> can be specified.
		///     It can expand / modify the artist information from a song.
		/// </summary>
		/// <param name="geniusApiKey">
		///     The API-key that will be used for the queries (i.e. to create the
		///     <see cref="GeniusClient" />).
		/// </param>
		/// <param name="overrideArtists">
		///     Determine whether the artists should be overriden (i.e. taken from genius - may result in completely wrong matched
		///     songs) or expanded / modified.
		/// </param>
		public GeniusSongInfoFetcher(string geniusApiKey, bool overrideArtists = true)
		{
			if (string.IsNullOrWhiteSpace(geniusApiKey))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(geniusApiKey));
			OverrideArtists = overrideArtists;

			GeniusClient = new GeniusClient(geniusApiKey);
		}

		/// <summary>
		///     Comnpare the given songinfo (<paramref name="currentInfo" />) with the genius database and extend it. It may not be
		///     modified (e.g. if no match could be found).
		/// </summary>
		/// <param name="currentInfo">
		///     The info that will be used as baseline. <see cref="SongInfo.Title" /> and
		///     <see cref="SongInfo.Artists" /> will be used for the query.
		/// </param>
		/// <returns>A new songinfo containing relevant info from https://genius.com.</returns>
		public virtual async Task<SongInfo> ExtractAsyncTask(SongInfo currentInfo)
		{
			return await Task.Run(async () =>
			{
				string searchTerm = currentInfo.Title;

				if (currentInfo.Artists.Length > 0)
					searchTerm = $"{currentInfo.Artists[0]} {searchTerm}";

				var result = (await GeniusClient.SearchClient.Search(TextFormat.Dom, searchTerm)).Response;

				var correctHit = (from hit in result where hit.Type.Equals("song") select (JObject) hit.Result).FirstOrDefault();

				if (correctHit != null)
				{
					long songId = correctHit.GetValue("id").Value<long>();

					var song = await GeniusClient.SongsClient.GetSong(TextFormat.Html, songId.ToString());
					currentInfo = await SetSong(currentInfo, song.Response);
				}

				return currentInfo;
			});
		}

		/// <summary>
		///     This method actually stores the info from a given song into a given songinfo.
		/// </summary>
		/// <param name="info">The info that will be used as a baseline. (A new one will be returned).</param>
		/// <param name="song">The song that will be used to add information to <paramref name="info" />.</param>
		/// <returns>A new songinfo containing relevant info from the <paramref name="song" />.</returns>
		protected virtual async Task<SongInfo> SetSong(SongInfo info, Song song)
		{
			// The pictures that should be fetched (if available)
			var desiredPictures = new List<GeniusPicture>
			{
				new GeniusPicture(song.SongArtImageUrl, PictureType.FrontCover)
			};

			if (DownloadArtistImage)
				desiredPictures.Add(new GeniusPicture(song.PrimaryArtist.ImageUrl, PictureType.Artist));

			desiredPictures = desiredPictures.Where(p => !string.IsNullOrWhiteSpace(p.Url)).ToList();

			// The tasks for the pictures to fetch
			// ReSharper disable once AccessToDisposedClosure
			var desiredPicturesTask = desiredPictures.Select(p => p.DownloadAsync()).ToList();

			if (!string.IsNullOrWhiteSpace(song.ReleaseDate))
			{
				var releaseDate = DateTime.ParseExact(song.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				info.Year = (uint) releaseDate.Year;
			}

			if (song.Album != null)
			{
				info.Album = song.Album?.Name;
				info.AlbumArtists = new[] { song.Album.Artist.Name };
			}

			var artists = new List<string>();

			if (OverrideTitle)
				info.Title = song.Title;

			if (OverrideArtists)
			{
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

			foreach (var task in desiredPicturesTask)
			{
				await task;
			}

			for (int i = 0; i < desiredPictures.Count; i++)
			{
				if (desiredPicturesTask[i].IsFaulted)
					desiredPictures.RemoveAt(i--);
			}

			// TODO: keep old images that have already been stored?
			info.Pictures = desiredPictures.Select(p => p.CreatePictureFrame()).ToArray();

			return info;
		}

		/// <inheritdoc />
		public virtual async Task<SongInfo> ExtractAsyncTask(IFileSource source) =>
			await ExtractAsyncTask(SongInfo.ReadFromFile(source.FileInfo.FullName));

		/// <summary>
		///     A genius picture that is used to easily download it
		/// </summary>
		protected struct GeniusPicture : IDisposable
		{
			/// <summary>
			///     The url of the image
			/// </summary>
			public string Url;

			/// <summary>
			///     A temporary path for the file, it will automatically be created.
			/// </summary>
			public string Path;

			/// <summary>
			///     The type of the picture, that will be set.
			/// </summary>
			public PictureType Type;

			private WebClient _client;

			/// <summary>
			///     Create a new picture from an url.
			/// </summary>
			/// <param name="url">The url that is used to identify the picture. This url can be empty.</param>
			/// <param name="type">The type of the picture, that will be set.</param>
			public GeniusPicture(string url, PictureType type)
			{
				Url = url;
				Path = System.IO.Path.GetTempFileName();
				Type = type;
				_client = null;
			}

			/// <summary>
			///     Get a task that is downloading the picture. If the <see cref="Url" /> is empty, <c>null</c> will be returned.
			/// </summary>
			/// <returns>The task / progress of the downloading process.</returns>
			public Task DownloadAsync()
			{
				if (_client != null)
					throw new InvalidOperationException("Concurrent operations are not supported.");
				_client = new WebClient();

				return string.IsNullOrWhiteSpace(Url) ? null : _client.DownloadFileTaskAsync(Url, Path);
			}

			/// <summary>
			///     Create a picture frame from the given iamge. Also dispose the tempfile of not otherwise sepcified.
			/// </summary>
			/// <param name="dispose">Whether to dispose the file after creating the picture.</param>
			/// <returns>A newly created picture frame that supports iTunes and other bitchy music players.</returns>
			public IPicture CreatePictureFrame(bool dispose = true)
			{
				var frame = SongInfo.CreatePictureFrame(Path, Type);

				if (dispose) Dispose();

				return frame;
			}

			/// <summary>
			///     Delete the downloaded image.
			/// </summary>
			public void Dispose()
			{
				if (!string.IsNullOrWhiteSpace(Url))
					File.Delete(Path);
				_client?.Dispose();
			}
		}
	}
}