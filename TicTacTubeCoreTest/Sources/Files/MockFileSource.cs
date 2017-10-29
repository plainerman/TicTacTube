using System.IO;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCoreTest.Sources.Files
{
	public class MockFileSource : IFileSource
	{
		public FileInfo FileInfo => null;
		public string FileName => null;
		public string FullFileName => null;
		public string FileExtension => null;
		public string Path => null;
	}
}