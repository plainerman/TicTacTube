using System;
using System.IO;

namespace TicTacTubeCore.Sources.Files
{
	/// <summary>
	///     The base implementation for any file source.
	/// </summary>
	public abstract class BaseFileSource : IFileSource
	{
		private IExternalFileSource _externalSource;

		/// <summary>
		///     Create a <see cref="BaseFileSource" /> from a given filepath. This cannot be changed.
		/// </summary>
		/// <param name="filePath">The absolute or relative filepath. This may not be <c>null</c> or empty.</param>
		protected BaseFileSource(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

			FileInfo = new FileInfo(filePath);
			FullFileName = FileInfo.Name;
			FileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
		}

		/// <summary>
		///     Create a <see cref="BaseFileSource" /> from a given external source that will be stored to the given local path.
		/// </summary>
		/// <param name="externalSource">The external source.</param>
		/// <param name="localPath">The local path this file will be stored to.</param>
		protected BaseFileSource(IExternalFileSource externalSource, string localPath) : this(localPath)
		{
			_externalSource = externalSource ?? throw new ArgumentNullException(nameof(externalSource));
		}

		/// <summary>
		/// Fetch the external source, if correctly fetched, set to <c>null</c>.
		/// </summary>
		protected void FetchExternalSource()
		{
			if (_externalSource == null) return;

			_externalSource.Fetch(System.IO.Path.Combine(Path, FullFileName));
			_externalSource = null;
		}

		/// <inheritdoc />
		public FileInfo FileInfo { get; }

		/// <inheritdoc />
		public string FileName { get; }

		/// <inheritdoc />
		public string FullFileName { get; }

		/// <inheritdoc />
		public string FileExtension => FileInfo?.Extension;

		/// <inheritdoc />
		public string Path => FileInfo?.Directory.FullName;
	}
}