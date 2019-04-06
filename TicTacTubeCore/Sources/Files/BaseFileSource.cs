using System;
using System.IO;
using log4net;
using TicTacTubeCore.Sources.Files.External;

namespace TicTacTubeCore.Sources.Files
{
	/// <summary>
	///     The base implementation for any file source.
	/// </summary>
	public abstract class BaseFileSource : IFileSource
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(BaseFileSource));
		private IExternalFileSource _externalSource;

		/// <summary>
		///     The local filepath used temporarily for external sources;
		/// </summary>
		private string _filePath;

		/// <summary>
		///     Create a <see cref="BaseFileSource" /> from a given filepath. This cannot be changed.
		/// </summary>
		/// <param name="filePath">The absolute or relative filepath. This may not be <c>null</c> or empty.</param>
		protected BaseFileSource(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

			AssignFilePath(filePath);
		}

		/// <summary>
		///     Create a <see cref="BaseFileSource" /> from a given external source that will be stored to the given local path.
		/// </summary>
		/// <param name="externalSource">The external source.</param>
		/// <param name="localPath">The local directory this file will be stored to.</param>
		protected BaseFileSource(IExternalFileSource externalSource, string localPath)
		{
			if (string.IsNullOrWhiteSpace(localPath))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(localPath));

			_externalSource = externalSource ?? throw new ArgumentNullException(nameof(externalSource));
			_filePath = localPath;

			if (!_externalSource.LazyLoading)
				FetchExternalSource(true);
		}

		/// <summary>
		///     Assign all local variables from the given filepath.
		/// </summary>
		/// <param name="filePath">The file path that will be analyzed.</param>
		private void AssignFilePath(string filePath)
		{
			FileInfo = new FileInfo(filePath);
			FullFileName = FileInfo.Name;
			FileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
		}

		/// <summary>
		///     Fetch the external source, if correctly fetched, set to <c>null</c>.
		///     If fetched asynchronously, the method will return immediately. It has to be called synchronously
		///     before actually using the external file source.
		/// </summary>
		/// <param name="async">Whether the external source will be fetched asynchronously or not.</param>
		protected virtual void FetchExternalSource(bool async)
		{
			if (_externalSource == null)
				return;

			if (!Directory.Exists(_filePath))
			{
				Directory.CreateDirectory(_filePath);
				Log.InfoFormat("Creating directory {0}", _filePath);
			}

			Log.InfoFormat("Fetching external source {0} to {1}" + (async ? " asynchronously." : "."),
				_externalSource, _filePath);

			if (async)
			{
				_externalSource.FetchFileAsync(_filePath);
			}
			else
			{
				string path = _externalSource.FetchFile(_filePath);

				AssignFilePath(path);

				Log.InfoFormat("Fetched external source {0} to {1}.", _externalSource, path);

				_externalSource = null;
				_filePath = null;
			}
		}

		/// <inheritdoc />
		public FileInfo FileInfo { get; private set; }

		/// <inheritdoc />
		public string FileName { get; private set; }

		/// <inheritdoc />
		public string FullFileName { get; private set; }

		/// <inheritdoc />
		public string FileExtension => FileInfo?.Extension;

		/// <inheritdoc />
		public string Path => FileInfo?.Directory?.FullName;

		/// <inheritdoc />
		public void Init()
		{
			FetchExternalSource(false);
		}

		/// <inheritdoc />
		public void BeginExecute()
		{
		}

		/// <inheritdoc />
		public void EndExecute()
		{
		}
	}
}