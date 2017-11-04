using System.IO;
using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Filesystem
{
	/// <summary>
	/// A data processor that can duplicate a given source.s
	/// </summary>
	public class SourceCloner : BaseDataProcessor
	{
		private readonly string _destinationPath;
		private readonly bool _workOnClone;
		private readonly bool _keepName;

		/// <summary>
		/// Create a source cloner that clones a source to a complete path (also rename the file).
		/// </summary>
		/// <param name="destinationPath">The complete new path (inclusive file name).</param>
		/// <param name="workOnClone">Determine whether work should on continue on the new source or not.</param>
		public SourceCloner(string destinationPath, bool workOnClone)
		{
			_destinationPath = destinationPath;
			_workOnClone = workOnClone;
			_keepName = false;
		}

		/// <summary>
		/// Create a source cloner that clones a source to another directory and keeps the name. 
		/// </summary>
		/// <param name="destinationFolder">The folder where the source will be moved to.</param>
		/// <param name="keepName">Just a field to indicate a different constructor.</param>
		/// <param name="workOnClone">Determine whether work should on continue on the new source or not.</param>
		public SourceCloner(string destinationFolder, bool workOnClone, bool keepName)
		{
			_destinationPath = destinationFolder;
			_workOnClone = workOnClone;
			_keepName = true;
		}

		/// <inheritdoc />
		public override IFileSource Execute(IFileSource fileSoure)
		{
			var dest = _keepName ? Path.Combine(_destinationPath, fileSoure.FileInfo.Name) : _destinationPath;

			var directory = Path.GetDirectoryName(dest);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			fileSoure.FileInfo.CopyTo(dest);

			return _workOnClone ? new FileSource(dest) : fileSoure;
		}
	}
}