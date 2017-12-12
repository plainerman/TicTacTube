namespace TicTacTubeCore.Processors.Media
{
	/// <summary>
	///     A class that contains some kind of media info. (like song name or movie resolution).
	/// </summary>
	public interface IMediaInfo
	{
		/// <summary>
		///     Write the given media info as tags to the file. Previously stored information will be overriden.
		///     If the format does not support tags, throw an exception.
		/// </summary>
		/// <param name="path">The path to the file where the tags will be written to.</param>
		void WriteToFile(string path);
	}
}