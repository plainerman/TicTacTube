using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Sources.Files.External;
using SystemPath = System.IO.Path;

namespace TicTacTubeCoreTest.Sources.Files
{
	public class TempFileSource : BaseFileSource
	{
		public TempFileSource() : base(SystemPath.GetTempFileName())
		{ }

		~TempFileSource()
		{
			FileInfo.Delete();
		}
	}
}