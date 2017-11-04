using TicTacTubeCore.Processors.Definitions;

namespace TicTacTubeCore.Processors.Builders
{
	/// <summary>
	///     A data processor builder that builds <see cref="IDataProcessor" /> based on a generic type.
	/// </summary>
	/// <typeparam name="T">The type that is used to create data processors.</typeparam>
	public class GenericDataProcessorBuilder<T> : BaseDataProcessorBuilder where T : IDataProcessor, new()
	{
		/// <inheritdoc />
		public override IDataProcessor Build()
		{
			return new T();
		}
	}
}