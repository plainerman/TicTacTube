using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Definitions
{
	/// <summary>
	///     A data processor that actually processes data.
	/// </summary>
	public interface IDataProcessor : IDataProcessorOrBuilder
	{
		/// <summary>
		///     Perform some kind of action on a given file source. If the source
		///     somehow changes, return a new source - otherwise, the same source can
		///     be returned.
		/// </summary>
		/// <param name="fileSource">The file source that will be processed.</param>
		/// <returns>The file source which will be used for the next pipeline step. May be a new one or the same reference.</returns>
		IFileSource Execute(IFileSource fileSource);
	}
}