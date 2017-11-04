using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using TicTacTubeCore.Processors;
using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Pipelines
{
	/// <summary>
	///     A pipeline that stores multiple processors that process / modify / ... some sort of data.
	/// </summary>
	public class DataPipeline : IDataPipeline
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(DataPipeline));

		/// <summary>
		///     The collection of data processor that are the actual pipeline processors.
		/// </summary>
		protected readonly ICollection<IDataProcessor> DataProcessors;

		/// <summary>
		///     Create a <see cref="DataPipeline" /> with a given collection of dataProcessors.
		/// </summary>
		/// <param name="dataProcessors">
		///     The collection of data processors that are the actual pipeline processors. May not be
		///     <c>null</c>.
		/// </param>
		public DataPipeline(ICollection<IDataProcessor> dataProcessors)
		{
			DataProcessors = dataProcessors ?? throw new ArgumentNullException(nameof(dataProcessors));
		}

		/// <summary>
		///     Create a <see cref="DataPipeline" /> with a given collection of dataProcessorsOrBuilders.
		/// </summary>
		/// <param name="dataProcessorsOrBuilder">
		///     The collection of data processors (or builders) that are the actual pipeline
		///     processors. May not be <c>null</c>.
		/// </param>
		public DataPipeline(IEnumerable<IDataProcessorOrBuilder> dataProcessorsOrBuilder)
		{
			DataProcessors = dataProcessorsOrBuilder?.Select(d => d.Build()).ToList() ??
							throw new ArgumentNullException(nameof(dataProcessorsOrBuilder));
		}

		/// <inheritdoc />
		public void Execute(IFileSource fileSource)
		{
			Log.Info($"Executing pipeline with filesource {fileSource.GetType().Name}");
			IFileSource prev = null;
			DataProcessors.Aggregate(fileSource, (current, processor) =>
			{
				if (current != prev)
				{
					current.Init();
					prev = current;
				}

				current.BeginExecute();

				var newSource = processor.Execute(current);

				current.EndExecute();

				return newSource;
			});
		}

		/// <summary>
		///     Check whether the object is a builder or not. (Hint: it is never a builder).
		/// </summary>
		public bool IsBuilder => false;

		/// <inheritdoc />
		public IDataPipeline Build()
		{
			return this;
		}
	}
}