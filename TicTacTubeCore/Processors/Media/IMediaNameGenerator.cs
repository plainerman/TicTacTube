namespace TicTacTubeCore.Processors.Media
{
	/// <summary>
	///     A generator that is capable of generating names for a given <see cref="IMediaInfo" />.
	/// </summary>
	public interface IMediaNameGenerator<in T> where T : IMediaInfo
	{
		/// <summary>
		///     This method correctly parses the given media info and returns the new file name (without extension or similar
		///     info).
		/// </summary>
		/// <param name="info">The media info that will be used to create a new named string.</param>
		/// <returns>The newly parsed file name.</returns>
		string Parse(T info);
	}
}