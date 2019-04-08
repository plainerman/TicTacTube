using System;
using System.Collections.Generic;
using System.Linq;
using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Logical
{
	/// <summary>
	///     A processor that is like a nested pipeline and also aggregates the file sources.
	/// </summary>
	public class MultiProcessor : BaseDataProcessor
	{
		/// <summary>
		///     Internally stored processors that will be executed.
		/// </summary>
		protected readonly IEnumerable<IDataProcessorOrBuilder> Processors;

		/// <summary>
		///     Create a new multi processor (that acts like a nested pipeline) with given processors.
		/// </summary>
		/// <param name="processor">The first processor that will be executed. This one is required.</param>
		/// <param name="processors">The processors that will be managed and processed. May be <code>null</code> or empty.</param>
		public MultiProcessor(IDataProcessorOrBuilder processor, params IDataProcessorOrBuilder[] processors)
		{
			var dataProcessorOrBuilders = new IDataProcessorOrBuilder[(processors?.Length ?? 0) + 1];
			dataProcessorOrBuilders[0] = processor ?? throw new ArgumentNullException(nameof(processor));

			if (processors != null)
			{
				Array.Copy(processors, 0, dataProcessorOrBuilders, 1, processors.Length);
			}

			Processors = dataProcessorOrBuilders;
		}

		/// <summary>
		///     Create a new multi processor (that acts like a nested pipeline) with given processors.
		/// </summary>
		/// <param name="processors">The processors that will be managed and processed.</param>
		public MultiProcessor(IEnumerable<IDataProcessorOrBuilder> processors)
		{
			if (processors == null) throw new ArgumentNullException(nameof(processors));

			var dataProcessorOrBuilders = processors as IDataProcessorOrBuilder[] ?? processors.ToArray();

			if (!dataProcessorOrBuilders.Any()) throw new ArgumentException("May not be empty.", nameof(processors));

			Processors = dataProcessorOrBuilders;
		}

		/// <inheritdoc />
		public override IFileSource Execute(IFileSource fileSource)
		{
			return Processors.Aggregate(fileSource,
				(current, dataProcessorOrBuilder) => dataProcessorOrBuilder.Build().Execute(current));
		}
	}
}