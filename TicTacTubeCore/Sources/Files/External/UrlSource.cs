using System;
using System.IO;
using System.Net;
using System.Net.Mime;

namespace TicTacTubeCore.Sources.Files.External
{
	/// <summary>
	///     An external source that fetches from an http url.
	/// </summary>
	public class UrlSource : BaseExternalFileSource
	{
		/// <summary>
		///     The url from which the file will be fetched.
		/// </summary>
		public string Url { get; }


		/// <inheritdoc />
		public override string ExternalSource => Url;

		/// <summary>
		///     Create a new <see cref="IExternalFileSource" /> that will be fetched from an url and define whether it is lazy
		///     loaded or not.
		/// </summary>
		/// <param name="url">The url from which the file will be fetched.</param>
		/// <param name="lazyLoading">If <c>true</c>, the file will be fetched as late as possible.</param>
		public UrlSource(string url, bool lazyLoading = false) : base(lazyLoading)
		{
			if (string.IsNullOrWhiteSpace(url))
				throw new ArgumentException("Value cannot be null or whitespace", nameof(url));

			Url = url;
		}

		/// <summary>
		///     This method returns the preferred filepath based on the given <see ref="destinationPath" /> and the default
		///     filename from the server.
		///     If the file already exists, it will fetch a name with <see cref="BaseExternalFileSource.GetAllowedFileName" />.
		/// </summary>
		/// <param name="client">The client that is used to get the correct file. (<paramref name="url" /> is used).</param>
		/// <param name="url">The url from which the filename will be fetched.</param>
		/// <param name="destinationPath">The base path that will be prepended to the filename.</param>
		/// <returns>The new path (destinationPath and the new filename).</returns>
		protected virtual string GetDownloadedFilePath(WebClient client, string url, string destinationPath)
		{
			client.OpenRead(url);

			string headerContentDisposition = client.ResponseHeaders["content-disposition"];

			string filename = string.IsNullOrEmpty(headerContentDisposition)
				? Path.GetFileName(url)
				: new ContentDisposition(headerContentDisposition).FileName;

			filename = GetAllowedFileName(destinationPath, filename);

			return Path.Combine(destinationPath, filename);
		}

		/// <inheritdoc />
		protected override void Download(string destinationPath)
		{
			using (var client = new WebClient())
			{
				FinishedPath = GetDownloadedFilePath(client, Url, destinationPath);

				client.DownloadFile(Url, FinishedPath);
			}
		}

		/// <inheritdoc />
		protected override void DownloadAsync(string destinationPath)
		{
			using (var client = new WebClient())
			{
				FinishedPath = GetDownloadedFilePath(client, Url, destinationPath);

				CurrentDownloadTask = client.DownloadFileTaskAsync(Url, FinishedPath);
			}
		}
	}
}