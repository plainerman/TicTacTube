using TicTacTubeCore.Processors.Builders;

namespace TicTacTubeCore.Processors
{
	/// <summary>
	/// A a builder capable of creating a data processor (see <see cref="IDataProcessorOrBuilder"/> and <see cref="IDataProcessor"/>).
	/// </summary>
	public abstract class BaseDataProcessorBuilder : IDataProcessorBuilder
	{
		/// <inheritdoc />
		public abstract IDataProcessor Build();
		/// <summary>
		///     Check whether the object is a builder or not. (Hint: it is always a builder).
		/// </summary>
		public bool IsBuilder => true;
	}
}