using System;
using System.IO;
using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Filesystem
{
	/// <inheritdoc />
	/// <summary>
	///     A data processor that can rename a given file.
	/// </summary>
	public class SourceRenamer : BaseDataProcessor
	{
		/// <summary>
		///     Whether an existing file will be overriden or not.
		/// </summary>
		protected readonly bool Override;

		/// <summary>
		///     A function that is capable of producing a new file name (with file extension) for a given file source. May not be
		///     <c>null</c>.
		/// </summary>
		public Func<IFileSource, string> NameProducer { get; protected set; }

		/// <inheritdoc />
		/// <summary>
		///     Create a new source renamer, that renames files with a given renamer.
		/// </summary>
		/// <param name="nameProducer">
		///     A function that is capable of producing a new file name (with file extension) for a given file source.
		///     If the new file name contains a '/' or a '\', it will also be moved to the produced folder (folder will be created
		///     if required).
		///     May not be <c>null</c>.
		/// </param>
		/// <param name="override"><c>True</c>, if an existing destination should be overriden.</param>
		public SourceRenamer(Func<IFileSource, string> nameProducer, bool @override = true) : this(@override)
		{
			NameProducer = nameProducer ?? throw new ArgumentNullException(nameof(nameProducer));
		}

		/// <summary>
		///     Create a new source renamer, that renames files with a given renamer. A <see cref="NameProducer" /> has to be
		///     manually defined.
		/// </summary>
		/// <param name="override"><c>True</c>, if an existing destination should be overriden.</param>
		protected SourceRenamer(bool @override = true)
		{
			Override = @override;
		}

		/// <inheritdoc />
		public override IFileSource Execute(IFileSource fileSource)
		{
			string newFileName = NameProducer(fileSource);
			string newFolder = Path.GetDirectoryName(newFileName);
			string fullPath = Path.Combine(fileSource.FileInfo.DirectoryName ?? "", newFileName);

			if (!string.IsNullOrEmpty(newFolder))
			{
				string newFolderFullPath = Path.Combine(Path.Combine(fileSource.FileInfo.DirectoryName), newFolder);

				if (!Directory.Exists(newFolderFullPath))
					Directory.CreateDirectory(newFolderFullPath);
			}

			if (Override && File.Exists(fullPath))
				File.Delete(fullPath);

			fileSource.FileInfo.MoveTo(fullPath);

			return new FileSource(fullPath);
		}
	}
}