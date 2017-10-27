using TicTacCore.Processors.Builders;

namespace TicTacCore.Processors
{
	public abstract class BaseDataProcessorBuilder : IDataProcessorBuilder
	{
		public abstract IDataProcessor Build();
		public bool IsBuilder => true;
	}
}