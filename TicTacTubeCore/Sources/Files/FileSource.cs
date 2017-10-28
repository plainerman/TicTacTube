namespace TicTacTubeCore.Sources.Files
{
	/// <summary>
	/// The default implementation of a file source.
	/// </summary>
	public class FileSource : BaseFileSource
	{
		/// <summary>
		/// Create a <see cref="FileSource"/> from a given filepath. This cannot be changed.
		/// </summary>
		/// <param name="filePath">The absolute or relative filepath. This may not be <c>null</c> or empty.</param>
		public FileSource(string filePath) : base(filePath)
		{
		}
	}
}