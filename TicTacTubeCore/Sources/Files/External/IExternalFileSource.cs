namespace TicTacTubeCore.Sources.Files.External
{
	/// <summary>
	///     An external file source (e.g. stored on a web server)
	/// </summary>
	public interface IExternalFileSource : IDataSource
	{
		/// <summary>
		///     Whether this file should be loaded asap or not.
		/// </summary>
		bool LazyLoading { get; }

		/// <summary>
		///     Fetch the file from the source.
		/// </summary>
		/// <param name="destinationPath"></param>
		/// <returns>The path of the newly downloaded file.</returns>
		string FetchFile(string destinationPath);
	}
}