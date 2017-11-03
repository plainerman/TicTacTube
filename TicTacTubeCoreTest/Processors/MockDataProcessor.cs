using TicTacTubeCore.Processors;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCoreTest.Processors
{
	public class MockDataProcessor : BaseDataProcessor
	{
		public override IFileSource Execute(IFileSource fileSoure)
		{
			return fileSoure;
		}
	}
}