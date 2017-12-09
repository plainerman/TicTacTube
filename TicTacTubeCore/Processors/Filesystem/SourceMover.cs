using System.IO;
using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Filesystem
{
	/// <summary>
	///     A data processer that can move a file source to another path.
	/// </summary>
	public class SourceMover : BaseDataProcessor
	{
		/// <summary>
		/// The destination path or folder.
		/// </summary>
		protected readonly string DestinationPath;
		/// <summary>
		/// <c>True</c>, if an existing destination should be overriden.
		/// </summary>
		protected readonly bool Override;
		/// <summary>
		/// Whether the name should be kept (<see cref="DestinationPath"/> is a folder) or one is specified (<see cref="DestinationPath"/> is a file).
		/// </summary>
		protected readonly bool KeepName;

		/// <summary>
		///     Create a source mover that moves a source to a complete path (also rename the file).
		/// </summary>
		/// <param name="destinationPath">The complete new path (inclusive file name).</param>
		/// <param name="override"><c>True</c>, if an existing destination should be overriden.</param>
		public SourceMover(string destinationPath, bool @override = true)
		{
			DestinationPath = destinationPath;
			Override = @override;
			KeepName = false;
		}

		/// <summary>
		///     Create a source mover that moves a source to another directory and keeps the name.
		/// </summary>
		/// <param name="destinationFolder">The folder where the source will be moved to.</param>
		/// <param name="keepName">Just a field to indicate a different constructor.</param>
		/// <param name="override"><c>True</c>, if an existing destination should be overriden.</param>
		public SourceMover(string destinationFolder, bool keepName, bool @override = true)
		{
			DestinationPath = destinationFolder;
			Override = @override;
			KeepName = true;
		}

		/// <inheritdoc />
		public override IFileSource Execute(IFileSource fileSoure)
		{
			string dest = KeepName ? Path.Combine(DestinationPath, fileSoure.FileInfo.Name) : DestinationPath;

			string directory = Path.GetDirectoryName(dest);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			if (Override && File.Exists(dest))
			{
				File.Delete(dest);
			}

			fileSoure.FileInfo.MoveTo(dest);

			return new FileSource(dest);
		}
	}
}