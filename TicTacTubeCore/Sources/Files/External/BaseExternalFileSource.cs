using System;
using System.IO;
using System.Threading.Tasks;

namespace TicTacTubeCore.Sources.Files.External
{
	/// <summary>
	///     The base implementation for an external file source.
	/// </summary>
	public abstract class BaseExternalFileSource : IExternalFileSource
	{
		/// <summary>
		///     The current async task that is downloading given url.
		/// </summary>
		protected Task CurrentDownloadTask;

		/// <summary>
		///     Tha path that will be assigned, once the file has downloaded for the first time.
		/// </summary>
		protected string FinishedPath;

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

		/// <summary>
		///     Download the given external source.
		/// </summary>
		/// <param name="destinationPath">The folder where the file will be downloaded and stored.</param>
		/// <returns>The path of the newly downloaded file.</returns>
		public string FetchFile(string destinationPath)
		{
			// If the download has already started.
			if (CurrentDownloadTask != null)
			{
				// Wait for the download to finish
				CurrentDownloadTask.Wait();

				// If there was an error with the task
				if (!CurrentDownloadTask.IsCompletedSuccessfully)
				{
					Exception e = CurrentDownloadTask.Exception;
					CurrentDownloadTask = null;

					throw e;
				}

				CurrentDownloadTask = null;

				return FinishedPath;
			}

			Download(destinationPath);

			return FinishedPath;
		}

		/// <inheritdoc />
		/// <summary>
		///     Download the given external source.
		/// </summary>
		/// <param name="destinationPath">The folder where the file will be downloaded and stored.</param>
		public void FetchFileAsync(string destinationPath)
		{
			DownloadAsync(destinationPath);
		}

		/// <summary>
		///     Actually download a given file synchronously. If correctly assigned, this method does autoamtically work with
		///     previously fetched asynchronous sources.
		///     <see cref="FinishedPath" /> has to be correctly set inside this method.
		/// </summary>
		/// <param name="destinationPath">The base path that will be prepended to the filename.</param>
		protected abstract void Download(string destinationPath);

		/// <summary>
		///     Actually download a givenf ile asynchronously.
		///     <see cref="FinishedPath" /> and <see cref="CurrentDownloadTask" /> have to be assigned correctly in
		///     order to automatically work with synchronous sources.
		/// </summary>
		/// <param name="destinationPath">The base path that will be prepended to the filename.</param>
		protected abstract void DownloadAsync(string destinationPath);

		/// <summary>
		///     Find a filename that is as close as possible to the desired filename.
		/// </summary>
		/// <param name="destinationPath">The path in wehich the desired file will be stored.</param>
		/// <param name="desiredFileName">The desired filename.</param>
		/// <returns>The new file name.</returns>
		protected virtual string GetAllowedFileName(string destinationPath, string desiredFileName)
		{
			if (string.IsNullOrWhiteSpace(desiredFileName))
			{
				desiredFileName = "download.dat";
			}

			string path = Path.Combine(destinationPath, desiredFileName);

			if (!File.Exists(path))
			{
				return desiredFileName;
			}

			int index = 1;
			while (true)
			{
				string currentDesiredFilename =
					$"{Path.GetFileNameWithoutExtension(desiredFileName)}_{index}{Path.GetExtension(desiredFileName)}";
				path = Path.Combine(destinationPath, currentDesiredFilename);

				if (!File.Exists(path))
				{
					return currentDesiredFilename;
				}

				index++;
			}
		}
	}
}