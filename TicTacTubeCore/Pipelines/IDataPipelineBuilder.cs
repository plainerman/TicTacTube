using System.Collections.Generic;
using TicTacTubeCore.Processors.Definitions;

namespace TicTacTubeCore.Pipelines
{
	/// <summary>
	///     A data pipeline builder that is capable of creating an actual data pipeline (see <see cref="IDataPipeline" />).
	/// </summary>
	public interface IDataPipelineBuilder : IDataPipelineOrBuilder
	{
		/// <summary>
		///     Determine whether this builder is locked or not.
		/// </summary>
		bool IsLocked { get; }

		/// <summary>
		///     The data processors of this data pipeline builder.
		/// </summary>
		IReadOnlyCollection<IDataProcessorOrBuilder> Pipeline { get; }

		/// <summary>
		///     Lock the builder, that the internally stored pipeline of data processor cannot be modified.
		/// </summary>
		/// <returns>The same data pipeline builder.</returns>
		IDataPipelineBuilder LockBuilder();

		/// <summary>
		///     Append a data processor.
		/// </summary>
		/// <param name="dataProcessorOrBuilder">Append a data processor that can actually process data.</param>
		/// <returns>The same data pipeline builder.</returns>
		IDataPipelineBuilder Append(IDataProcessorOrBuilder dataProcessorOrBuilder);
	}
}