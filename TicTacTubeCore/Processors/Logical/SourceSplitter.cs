using System;
using System.Collections.Generic;
using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Logical
{
	/// <summary>
	///     This source splitter is capable of applying multiple processors to one source.
	/// </summary>
	public class SourceSplitter : BaseDataProcessor
	{
		/// <summary>
		///     The processors that will be executed.
		/// </summary>
		protected readonly IEnumerable<IDataProcessorOrBuilder> Processors;

		/// <summary>
		///     Create a new source splitter that passes the given file source (the one from execute) to multiple params.
		///     If there is a following data processor in the pipeline, it receives an unmodified version of the filesource.
		/// </summary>
		/// <param name="processors">The processors that will be executed.</param>
		public SourceSplitter(IEnumerable<IDataProcessorOrBuilder> processors)
		{
			Processors = processors ?? throw new ArgumentNullException(nameof(processors));
		}

		/// <inheritdoc />
		public override IFileSource Execute(IFileSource fileSource)
		{
			foreach (var dataProcessorOrBuilder in Processors)
			{
				dataProcessorOrBuilder.Build().Execute(fileSource);
			}

			return fileSource;
		}
	}
}