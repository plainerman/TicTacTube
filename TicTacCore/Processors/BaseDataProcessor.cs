namespace TicTacTubeCore.Processors
{
	public abstract class BaseDataProcessor : IDataProcessor
	{
		public IDataProcessor Build()
		{
			return this;
		}

		public bool IsBuilder => false;
	}
}