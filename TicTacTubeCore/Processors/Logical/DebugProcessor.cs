using System;
using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Logical
{
	/// <summary>
	///     A debug processor that wraps any given processor and may make debugging easier.
	/// </summary>
	public class DebugProcessor : BaseDataProcessor
	{
		/// <summary>
		///     The internally wrapped data processor.
		/// </summary>
		public IDataProcessorOrBuilder DataProcessor { get; }

		/// <summary>
		///     The amount of calls on execute.
		/// </summary>
		public int ExecutionCount { get; protected set; }

		/// <summary>
		///     Create a new debug processor that wraps the given data processor.
		/// </summary>
		/// <param name="dataProcessor">The data processor that will actually be executed.</param>
		public DebugProcessor(IDataProcessorOrBuilder dataProcessor)
		{
			DataProcessor = dataProcessor ?? throw new ArgumentNullException(nameof(dataProcessor));
		}

		/// <inheritdoc />
		public override IFileSource Execute(IFileSource fileSoure)
		{
			var newFileSource = DataProcessor.Build().Execute(fileSoure);
			ExecutionCount++;

			return newFileSource;
		}
	}
}