﻿using log4net;
using System;
using System.IO;
using TicTacTubeCore.Sources.Files.External;

namespace TicTacTubeCore.Sources.Files
{
	/// <summary>
	///     The base implementation for any file source.
	/// </summary>
	public abstract class BaseFileSource : IFileSource
	{
		private IExternalFileSource _externalSource;

		private static readonly ILog Log = LogManager.GetLogger(typeof(BaseFileSource));

		/// <summary>
		/// The local filepath used temporarily for external sources;
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

		private void AssignFilePath(string filePath)
		{
			FileInfo = new FileInfo(filePath);
			FullFileName = FileInfo.Name;
			FileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
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
			{
				FetchExternalSource();
			}
		}

		/// <summary>
		/// Fetch the external source, if correctly fetched, set to <c>null</c>.
		/// </summary>
		protected virtual void FetchExternalSource()
		{
			if (_externalSource == null) return;

			if (!Directory.Exists(_filePath))
			{
				Directory.CreateDirectory(_filePath);
				Log.Info($"Creating directory {_filePath}");
			}

			Log.Info($"Fetching external source {_externalSource} to {_filePath}.");

			string path = _externalSource.FetchFile(_filePath);

			AssignFilePath(path);

			Log.Info($"Fetched external source {_externalSource} to {path}.");

			_externalSource = null;
			_filePath = null;
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
		public string Path => FileInfo?.Directory.FullName;
	}
}