using System.Threading.Tasks;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Media
{
	/// <summary>
	///     An extractor that is capable of generating MediaInfo from a given <c>string</c>.
	/// </summary>
	public interface IMediaTextInfoExtractor<T> where T : IMediaInfo
	{

		/// <summary>
		///     This method extracts a mediainfo from a given string (<paramref name="source" />).
		/// </summary>
		/// <param name="source">
		///     The string that should be as verbose as possible for the program to correctly identify the media.
		///		This string may be an url or the song title itself or as specified in the implementing class.
		/// </param>
		/// <returns>A <see cref="IMediaInfo" /> containing the extracted information.</returns>
		Task<T> ExtractFromStringAsyncTask(string source);
	}
}