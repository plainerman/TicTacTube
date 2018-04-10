using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeTest.Processors
{
	public class MockDataProcessor : BaseDataProcessor
	{
		public int ExecutionCount { get; protected set; }

		public override IFileSource Execute(IFileSource fileSoure)
		{
			ExecutionCount++;
			return fileSoure;
		}

		public void ResetExecutionCount() => ExecutionCount = 0;
	}
}