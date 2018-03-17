using System.IO;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Sources.Files.External;

namespace TicTacTubeCoreTest.Sources.Files
{
	public class MockFileSource : IFileSource
	{
		public FileInfo FileInfo => null;
		public string FileName => null;
		public string FullFileName => null;
		public string FileExtension => null;
		public IExternalFileSource ExternalFileSource => null;
		public string Path => null;

		public void Init()
		{
		}

		public void BeginExecute()
		{
		}

		public void EndExecute()
		{
		}
	}
}