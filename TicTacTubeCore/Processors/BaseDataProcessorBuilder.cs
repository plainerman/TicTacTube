using TicTacTubeCore.Processors.Builders;

namespace TicTacTubeCore.Processors
{
	public abstract class BaseDataProcessorBuilder : IDataProcessorBuilder
	{
		public abstract IDataProcessor Build();
		public bool IsBuilder => true;
	}
}