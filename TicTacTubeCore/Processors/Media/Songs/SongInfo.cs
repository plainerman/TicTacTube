using System;
using System.IO;
using System.Threading.Tasks;
using TagLib;
using TagLib.Id3v2;
using File = TagLib.File;

namespace TicTacTubeCore.Processors.Media.Songs
{
	/// <summary>
	///     Info for a given song.
	/// </summary>
	public struct SongInfo : IMediaInfo
	{
		/// <summary>
		///     The title of the song.
		/// </summary>
		public string Title;

		/// <summary>
		///     The contributing artists.
		/// </summary>
		public string[] Artists;

		/// <summary>
		///     The album of the song.
		/// </summary>
		public string Album;

		/// <summary>
		///     The album artists from the album;
		/// </summary>
		public string[] AlbumArtists;

		/// <summary>
		///     The genres of the song.
		/// </summary>
		public string[] Genres;

		/// <summary>
		///     The year this song was released.
		/// </summary>
		public uint Year;

		/// <summary>
		///     The bitrate in kbps.
		/// </summary>
		public int Bitrate;

		/// <summary>
		///     The pictures that belong to the song (e.g. cover art).
		/// </summary>
		public IPicture[] Pictures;

		//TODO:lyrics?
		/// <inheritdoc />
		public void WriteToFile(string path)
		{
			using (var f = File.Create(path))
			{
				f.Tag.Title = Title;
				f.Tag.Performers = Artists;
				f.Tag.Album = Album;
				f.Tag.AlbumArtists = AlbumArtists;
				f.Tag.Genres = Genres;
				f.Tag.Year = Year;
				f.Tag.Pictures = Pictures;

				f.Tag.Comment = "";

				f.Save();
			}
		}

		/// <summary>
		///     Read the songinfo from a given file.
		/// </summary>
		/// <param name="path">The path from which the information will be read.</param>
		/// <returns>The extracted songinfo from the file.</returns>
		public static SongInfo ReadFromFile(string path)
		{
			var songInfo = new SongInfo();

			using (var f = File.Create(path))
			{
				songInfo.Title = f.Tag.Title;
				songInfo.Artists = f.Tag.Performers;
				songInfo.Album = f.Tag.Album;
				songInfo.AlbumArtists = f.Tag.AlbumArtists;
				songInfo.Genres = f.Tag.Genres;
				songInfo.Year = f.Tag.Year;
				songInfo.Bitrate = f.Properties.AudioBitrate;
				songInfo.Pictures = f.Tag.Pictures;
			}

			return songInfo;
		}

		/// <summary>
		///     Read the songinfo from a given file asynchronously.
		/// </summary>
		/// <param name="path">The path from which the information will be read.</param>
		/// <returns>The extracted songinfo from the file.</returns>
		public static async Task<SongInfo> ReadFromFileAsyncTask(string path) => await Task.Run(() => ReadFromFile(path));

		/// <summary>
		///     Create a picture frame from the given iamge—meaning that the picture can be displayed in apps like iTunes or Google Play Music.
		/// </summary>
		/// <param name="path">The path to the image.</param>
		/// <param name="type">The type this picture is.</param>
		/// <returns>A newly created picture frame that supports iTunes and other bitchy music players.</returns>
		public static IPicture CreatePictureFrame(string path, PictureType type)
		{
			return new AttachedPictureFrame(new Picture(path)) { TextEncoding = StringType.Latin1, Type = type };
		}
	}
}