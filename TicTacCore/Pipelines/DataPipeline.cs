using System;
using System.Collections.Generic;
using System.Linq;
using TicTacCore.Processors;

namespace TicTacCore.Pipelines
{
	public class DataPipeline : IDataPipeline
	{
		protected readonly ICollection<IDataProcessor> DataProcessors;

		public DataPipeline(ICollection<IDataProcessor> dataProcessors)
		{
			DataProcessors = dataProcessors ?? throw new ArgumentNullException(nameof(dataProcessors));
		}

		public DataPipeline(IEnumerable<IDataProcessorOrBuilder> dataProcessorsOrBuilder)
		{
			DataProcessors = dataProcessorsOrBuilder?.Select(d => d.Build()).ToList() ??
							throw new ArgumentNullException(nameof(dataProcessorsOrBuilder));
		}
	}
}