namespace TicTacTubeCore.Sources.Files.External
{
	/// <summary>
	///     The base implementation for an external file source.
	/// </summary>
	public abstract class BaseExternalFileSource : IExternalFileSource
	{
		/// <summary>
		///     Create a new <see cref="IExternalFileSource" /> and define whether it is lazy loaded or not.
		/// </summary>
		/// <param name="lazyLoading">If <c>true</c>, the file will be fetched as late as possible.</param>
		protected BaseExternalFileSource(bool lazyLoading)
		{
			LazyLoading = lazyLoading;
		}

		/// <inheritdoc />
		public bool LazyLoading { get; }

		/// <inheritdoc />
		public abstract string FetchFile(string destinationPath);

		/// <inheritdoc />
		public abstract void FetchFileAsync(string destinationPath);
	}
}