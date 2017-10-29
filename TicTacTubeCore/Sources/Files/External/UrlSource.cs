using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace TicTacTubeCore.Sources.Files.External
{
	/// <summary>
	///     An external source that fetches from an http url.
	/// </summary>
	public class UrlSource : BaseExternalFileSource
	{
		/// <summary>
		///     The current async task that is downloading given url.
		/// </summary>
		private Task _currentDownloadTask;

		/// <summary>
		///     Tha path that will be assigned, once the file has downloaded for the first time.
		/// </summary>
		private string _finishedPath;

		/// <summary>
		///     Create a new <see cref="IExternalFileSource" /> that will be fetched from an url and define whether it is lazy
		///     loaded or not.
		/// </summary>
		/// <param name="url">The url from which the file will be fetched.</param>
		/// <param name="lazyLoading">If <c>true</c>, the file will be fetched as late as possible.</param>
		public UrlSource(string url, bool lazyLoading = false) : base(lazyLoading)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				throw new ArgumentException("Value cannot be null or whitespace", nameof(url));
			}

			Url = url;
		}

		/// <summary>
		///     The url from which the file will be fetched.
		/// </summary>
		public string Url { get; }

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

		/// <summary>
		///     This method returns the preferred filepath based on the given <see ref="destinationPath" /> and the default
		///     filename from the server.
		///     If the file already exists, it will fetch a name with <see cref="GetAllowedFileName" />.
		/// </summary>
		/// <param name="client">The client that is used to get the correct file. (<see cref="Url" /> is used).</param>
		/// <param name="destinationPath">The base path that will be prepended to the filename.</param>
		/// <returns>The new path (destinationPath and the new filename).</returns>
		protected virtual string GetDownloadedFilePath(WebClient client, string destinationPath)
		{
			client.OpenRead(Url);

			string headerContentDisposition = client.ResponseHeaders["content-disposition"];
			string filename = new ContentDisposition(headerContentDisposition).FileName;

			filename = GetAllowedFileName(destinationPath, filename);

			return Path.Combine(destinationPath, filename);
		}

		/// <summary>
		///     Download the given <see cref="Url" /> with the provided name (from the webserver).
		/// </summary>
		/// <param name="destinationPath">The folder where the file will be downloaded and stored.</param>
		/// <returns>The path of the newly downloaded file.</returns>
		public override string FetchFile(string destinationPath)
		{
			// If the download has already started.
			if (_currentDownloadTask != null)
			{
				// Wait for the download to finish
				_currentDownloadTask.Wait();

				// If there was an error with the task
				if (!_currentDownloadTask.IsCompletedSuccessfully)
				{
					Exception e = _currentDownloadTask.Exception;
					_currentDownloadTask = null;

					throw e;
				}

				_currentDownloadTask = null;

				return _finishedPath;
			}

			using (var client = new WebClient())
			{
				_finishedPath = GetDownloadedFilePath(client, destinationPath);

				client.DownloadFile(Url, _finishedPath);
			}

			return _finishedPath;
		}

		/// <summary>
		///     Download the given <see cref="Url" /> with the provided name (from the webserver).
		/// </summary>
		/// <param name="destinationPath">The folder where the file will be downloaded and stored.</param>
		public override void FetchFileAsync(string destinationPath)
		{
			using (var client = new WebClient())
			{
				_finishedPath = GetDownloadedFilePath(client, destinationPath);

				_currentDownloadTask = client.DownloadFileTaskAsync(Url, _finishedPath);
			}
		}
	}
}