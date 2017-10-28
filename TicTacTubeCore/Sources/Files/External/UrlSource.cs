using System.IO;
using System.Net;
using System.Net.Mime;

namespace TicTacTubeCore.Sources.Files.External
{
	/// <summary>
	/// An external source that fetches from an http url.
	/// </summary>
	public class UrlSource : BaseExternalFileSource
	{
		/// <summary>
		/// The url from which the file will be fetched.
		/// </summary>
		public string Url { get; }

		/// <summary>
		/// Create a new <see cref="IExternalFileSource"/> that will be fetched from an url and define whether it is lazy loaded or not.
		/// </summary>
		/// <param name="url">The url from which the file will be fetched.</param>
		/// <param name="lazyLoading">If <c>true</c>, the file will be fetched as late as possible.</param>
		public UrlSource(string url, bool lazyLoading = false) : base(lazyLoading)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				throw new System.ArgumentException("Value cannot be null or whitespace", nameof(url));
			}

			Url = url;
		}

		/// <summary>
		/// Download the given <see cref="Url"/> with the provided name (from the webserver).
		/// </summary>
		/// <param name="destinationPath">The folder where the file will be downloaded and stored.</param>
		/// <returns>The path of the newly downloaded file.</returns>
		public override string FetchFile(string destinationPath)
		{
			string path;

			using (var client = new WebClient())
			{
				client.OpenRead(Url);

				string headerContentDisposition = client.ResponseHeaders["content-disposition"];
				string filename = new ContentDisposition(headerContentDisposition).FileName;

				if (string.IsNullOrWhiteSpace(filename))
				{
					filename = "download.dat";
				}

				path = Path.Combine(destinationPath, filename);

				int index = 1;
				while (File.Exists(path))
				{
					path = Path.Combine(destinationPath, $"{Path.GetFileNameWithoutExtension(filename)}_{index}{Path.GetExtension(filename)}");
					index++;
				}

				client.DownloadFile(Url, path);
			}

			return path;
		}
	}
}