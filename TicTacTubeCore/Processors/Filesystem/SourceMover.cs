using System;
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
		private readonly Func<IFileSource, string> _processingFunction;
		private readonly string _destinationPath;
		private readonly bool _keepName;

		/// <summary>
		///     Create a source mover that moves a source to a complete path (also rename the file).
		/// </summary>
		/// <param name="destinationPath">The complete new path (inclusive file name).</param>
		public SourceMover(string destinationPath)
		{
			_destinationPath = destinationPath;
			_keepName = false;
		}

		/// <summary>
		///     Create a source mover that moves a source to another directory and keeps the name.
		/// </summary>
		/// <param name="destinationFolder">The folder where the source will be moved to.</param>
		/// <param name="keepName">Just a field to indicate a different constructor.</param>
		public SourceMover(string destinationFolder, bool keepName)
		{
			_destinationPath = destinationFolder;
			_keepName = true;
		}

		/// <summary>
		///		Create a source mover that moves a source to another directory. The full
		///		path to the new file is created by the processing function.
		/// </summary>
		/// <param name="processingFunction">The processing function returns for a given <see cref="IFileSource"/>
		///		a full path to the new file.
		/// </param>
		public SourceMover(Func<IFileSource, string> processingFunction)
		{
			_processingFunction = processingFunction ?? throw new ArgumentNullException(nameof(processingFunction));
		}

		/// <inheritdoc />
		public override IFileSource Execute(IFileSource fileSoure)
		{
			string dest = _destinationPath;

			if (_processingFunction != null)
			{
				dest = _processingFunction(fileSoure);
			}
			else if (_keepName)
			{
				dest = Path.Combine(_destinationPath, fileSoure.FileInfo.Name);
			}

			string directory = Path.GetDirectoryName(dest);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			fileSoure.FileInfo.MoveTo(dest);

			return new FileSource(dest);
		}
	}
}