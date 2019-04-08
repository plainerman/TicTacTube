using System.IO;
using log4net.Util;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Sources.Files.External;

namespace TicTacTubeTest.Sources.Files
{
	public class MockFileSource : IFileSource
	{
		public IExternalFileSource ExternalSource => null;
		public FileInfo FileInfo => null;
		public string FileName => null;
		public string FullFileName => null;
		public string FileExtension => null;
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