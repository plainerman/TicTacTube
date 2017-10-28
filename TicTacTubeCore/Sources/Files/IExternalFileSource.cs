namespace TicTacTubeCore.Sources.Files
{
	/// <summary>
	///     An external file source (e.g. stored on a web server)
	/// </summary>
	public interface IExternalFileSource : IDataSource
	{
		/// <summary>
		///     Whether this file should be loaded asap or not.
		/// </summary>
		bool LazyLoading { get; set; }

		/// <summary>
		///     Fetch the file from the source.
		/// </summary>
		/// <param name="destinationPath"></param>
		void Fetch(string destinationPath);
	}
}