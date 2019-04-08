﻿using System;
using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Logical
{
	/// <summary>
	///     A lambda processor. It can process any file source by a specified function.
	/// </summary>
	public class LambdaProcessor : BaseDataProcessor
	{
		/// <summary>
		///     The processor that processes the <see cref="IFileSource" />.
		/// </summary>
		protected Func<IFileSource, IFileSource> Processor { get; }

		/// <summary>
		///     Create a new lambda processor with a single function that completely defines the processor.
		/// </summary>
		/// <param name="processor">The processor that processes the <see cref="IFileSource" />.</param>
		public LambdaProcessor(Func<IFileSource, IFileSource> processor)
		{
			Processor = processor ?? throw new ArgumentNullException(nameof(processor));
		}

		/// <summary>
		///     Create a new lambda processor with a single action that completely defines the processor.
		///		The source is not modified.
		/// </summary>
		/// <param name="processor">The processor that processes the <see cref="IFileSource" />.</param>
		public LambdaProcessor(Action<IFileSource> processor)
		{
			if (processor == null) throw new ArgumentNullException(nameof(processor));
			Processor = source =>
			{
				processor(source);
				return source;
			};
		}

		/// <inheritdoc />
		public override IFileSource Execute(IFileSource fileSource) => Processor(fileSource);
	}
}