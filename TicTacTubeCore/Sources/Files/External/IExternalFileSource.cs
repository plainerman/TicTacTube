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
		///     Fetch the file from the source. If it has already been fetched asnychronously,
		///     this method waits for the download to finish and then returns the new file path.
		/// </summary>
		/// <param name="destinationPath">The folder where the file should be stored.</param>
		/// <returns>The path of the newly downloaded file.</returns>
		string FetchFile(string destinationPath);

		/// <summary>
		///     Fetch the file from the source asynchronously. See <see cref="FetchFile" />
		///     to get the path to the downloaded file.
		/// </summary>
		/// <param name="destinationPath">The folder where the file should be stored.</param>
		void FetchFileAsync(string destinationPath);

		/// <summary>
		///		The external source that this source references to.
		/// </summary>
		string ExternalSource { get; }
	}
}