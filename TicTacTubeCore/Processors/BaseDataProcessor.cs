namespace TicTacTubeCore.Processors
{
	/// <summary>
	///     A data processor that actually processes data.
	/// </summary>
	public abstract class BaseDataProcessor : IDataProcessor
	{
		/// <inheritdoc />
		public IDataProcessor Build()
		{
			return this;
		}

		/// <summary>
		///     Check whether the object is a builder or not. (Hint: it is never a builder).
		/// </summary>
		public bool IsBuilder => false;
	}
}