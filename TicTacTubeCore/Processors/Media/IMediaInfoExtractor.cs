using System.Threading.Tasks;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Media
{
	/// <summary>
	///     An extractor that is capable of generating MediaInfo from a given <see cref="IFileSource" />.
	/// </summary>
	public interface IMediaInfoExtractor<T> where T : IMediaInfo
	{
		/// <summary>
		///     Extract a given <see cref="IMediaInfo" /> from a given file source asynchronously..
		/// </summary>
		/// <param name="source">The file source that will be analyzed.</param>
		/// <returns>The newly created <see cref="IMediaInfo" />.</returns>
		Task<T> ExtractAsyncTask(IFileSource source);
	}
}