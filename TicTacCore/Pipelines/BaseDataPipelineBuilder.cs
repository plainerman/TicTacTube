using System;
using System.Collections.Generic;
using TicTacCore.Pipelines.Exceptions;
using TicTacCore.Processors;

namespace TicTacCore.Pipelines
{
	/// <summary>
	///     The default implementation for a pipeline builder (see <see cref="IDataPipelineBuilder" />).
	/// </summary>
	public abstract class BaseDataPipelineBuilder : IDataPipelineBuilder
	{
		/// <summary>
		///     The actual internally stored pipeline of data processors.
		/// </summary>
		protected readonly List<IDataProcessorOrBuilder> InternalPipeline;

		protected BaseDataPipelineBuilder()
		{
			InternalPipeline = new List<IDataProcessorOrBuilder>();
			Pipeline = InternalPipeline.AsReadOnly();
		}

		public IReadOnlyCollection<IDataProcessorOrBuilder> Pipeline { get; }

		/// <summary>
		///     Determines whether this builder is locked or not.
		/// </summary>
		public bool IsLocked { get; private set; }

		// todo: data pipeline source

		public IDataPipelineBuilder Append(IDataProcessorOrBuilder dataProcessorOrBuilder)
		{
			if (dataProcessorOrBuilder == null) throw new ArgumentNullException(nameof(dataProcessorOrBuilder));
			if (IsLocked) throw new PipelineStateException("The pipeline is locked and cannot be modified.");

			InternalPipeline.Add(dataProcessorOrBuilder);

			return this;
		}

		public IDataPipelineBuilder LockBuilder()
		{
			IsLocked = true;

			return this;
		}


		public abstract IDataPipeline Build();
	}
}