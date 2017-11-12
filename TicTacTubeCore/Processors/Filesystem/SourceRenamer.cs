using System;
using System.IO;
using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Filesystem
{
	/// <summary>
	/// A data processor that can rename a given file.
	/// </summary>
	public class SourceRenamer : BaseDataProcessor
	{
		/// <summary>
		/// A function that is capable of producing a new file name (with file extension) for a given file source. May not be <c>null</c>.
		/// </summary>
		public Func<IFileSource, string> NameProducer { get; protected set; }

		/// <summary>
		/// Create a new source renamer, that renames files with a given renamer.
		/// </summary>
		/// <param name="nameProducer">A function that is capable of producing a new file name (with file extension) for a given file source.
		/// May not be <c>null</c>.</param>
		public SourceRenamer(Func<IFileSource, string> nameProducer) : this()
		{
			NameProducer = nameProducer ?? throw new ArgumentNullException(nameof(nameProducer));
		}

		/// <summary>
		/// Create a new source renamer, that renames files with a given renamer. A <see cref="NameProducer"/> has to be manually defined.
		/// </summary>
		protected SourceRenamer() { }

		/// <inheritdoc />
		public override IFileSource Execute(IFileSource fileSoure)
		{
			string newFileName = NameProducer(fileSoure);
			string fullPath = Path.Combine(fileSoure.FileInfo.DirectoryName, newFileName);

			fileSoure.FileInfo.MoveTo(fullPath);

			return new FileSource(fullPath);
		}
	}
}