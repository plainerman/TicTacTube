using System;
using System.Collections.Generic;
using System.Linq;
using TicTacTubeCore.Processors;

namespace TicTacTubeCore.Pipelines
{
	/// <summary>
	///     A pipeline that stores multiple processors that process / modify / ... some sort of data.
	/// </summary>
	public class DataPipeline : IDataPipeline
	{
		/// <summary>
		/// The collection of data processor that are the actual pipeline processors.
		/// </summary>
		protected readonly ICollection<IDataProcessor> DataProcessors;

		/// <summary>
		/// Create a <see cref="DataPipeline"/> with a given collection of dataProcessors. 
		/// </summary>
		/// <param name="dataProcessors">The collection of data processors that are the actual pipeline processors. May not be <c>null</c>.</param>
		public DataPipeline(ICollection<IDataProcessor> dataProcessors)
		{
			DataProcessors = dataProcessors ?? throw new ArgumentNullException(nameof(dataProcessors));
		}

		/// <summary>
		/// Create a <see cref="DataPipeline"/> with a given collection of dataProcessorsOrBuilders.
		/// </summary>
		/// <param name="dataProcessorsOrBuilder">The collection of data processors (or builders) that are the actual pipeline processors. May not be <c>null</c>.</param>
		public DataPipeline(IEnumerable<IDataProcessorOrBuilder> dataProcessorsOrBuilder)
		{
			DataProcessors = dataProcessorsOrBuilder?.Select(d => d.Build()).ToList() ??
							throw new ArgumentNullException(nameof(dataProcessorsOrBuilder));
		}
	}
}