using System;
using System.IO;

namespace TicTacTubeCore.Sources.Files
{
	/// <summary>
	///     The base implementation for any file source.
	/// </summary>
	public abstract class BaseFileSource : IFileSource
	{
		protected BaseFileSource(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

			FileInfo = new FileInfo(filePath);
			FullFileName = FileInfo.Name;
			FileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
		}

		public FileInfo FileInfo { get; }

		public string FileName { get; }
		public string FullFileName { get; }
		public string FileExtension => FileInfo.Extension;
		public string Path => FileInfo.Directory.FullName;
	}
}