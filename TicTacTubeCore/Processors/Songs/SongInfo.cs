using TicTacTubeCore.Processors.Media;

namespace TicTacTubeCore.Processors.Songs
{
	/// <summary>
	/// Info for a given song.
	/// </summary>
	public struct SongInfo : IMediaInfo
	{
		/// <summary>
		/// The title of the song.
		/// </summary>
		public string Title;

		/// <summary>
		/// The contributing artists.
		/// </summary>
		public string[] Artists;

		/// <summary>
		/// The album of the song.
		/// </summary>
		public string Album;

		/// <summary>
		/// The genres of the song.
		/// </summary>
		public string[] Genres;

		/// <summary>
		/// The year this song was released.
		/// </summary>
		public string Year;

		//TOOD: cover art
	}
}