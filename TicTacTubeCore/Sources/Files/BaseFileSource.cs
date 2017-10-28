using System;
using System.IO;

namespace TicTacTubeCore.Sources.Files
{
	/// <summary>
	///     The base implementation for any file source.
	/// </summary>
	public abstract class BaseFileSource : IFileSource
	{
		/// <summary>
		/// Create a <see cref="BaseFileSource"/> from a given filepath. This cannot be changed.
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

		/// <inheritdoc />
		public FileInfo FileInfo { get; }

		/// <inheritdoc />
		public string FileName { get; }
		/// <inheritdoc />
		public string FullFileName { get; }
		/// <inheritdoc />
		public string FileExtension => FileInfo.Extension;
		/// <inheritdoc />
		public string Path => FileInfo.Directory.FullName;
	}
}