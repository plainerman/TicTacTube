using System.IO;
using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Filesystem
{
	/// <summary>
	///     A data processor that can delete a source.
	/// </summary>
	public class SourceDeleter : BaseDataProcessor
	{
		/// <summary>
		///     Delete the passed file source.
		/// </summary>
		/// <param name="fileSoure">The file source that will be deleted.</param>
		/// <returns>This method always returns <c>null</c>.</returns>
		public override IFileSource Execute(IFileSource fileSoure)
		{
			File.Delete(fileSoure.FileInfo.FullName);

			return null;
		}
	}
}