using System;
using System.Collections.Generic;
using TicTacTubeCore.Pipelines.Exceptions;
using TicTacTubeCore.Processors.Definitions;

namespace TicTacTubeCore.Pipelines
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

		/// <summary>
		///     The default constructor.
		/// </summary>
		protected BaseDataPipelineBuilder()
		{
			InternalPipeline = new List<IDataProcessorOrBuilder>();
			Pipeline = InternalPipeline.AsReadOnly();
		}

		/// <inheritdoc />
		public IReadOnlyCollection<IDataProcessorOrBuilder> Pipeline { get; }

		/// <summary>
		///     Determines whether this builder is locked or not.
		/// </summary>
		public bool IsLocked { get; private set; }

		// todo: data pipeline source?

		/// <summary>
		///     Append a dataprocessor or builder. This can only be done, if the pipeline is not locked.
		/// </summary>
		/// <param name="dataProcessorOrBuilder">The processor or builder that will be added.</param>
		/// <returns>The builder itself.</returns>
		public IDataPipelineBuilder Append(IDataProcessorOrBuilder dataProcessorOrBuilder)
		{
			if (dataProcessorOrBuilder == null)
			{
				throw new ArgumentNullException(nameof(dataProcessorOrBuilder));
			}
			if (IsLocked)
			{
				throw new PipelineStateException("The pipeline is locked and cannot be modified.");
			}

			InternalPipeline.Add(dataProcessorOrBuilder);

			return this;
		}

		/// <summary>
		///     Lock the builder (i.e. forbid modification of the pipeline).
		/// </summary>
		/// <returns>The builder itself.</returns>
		public IDataPipelineBuilder LockBuilder()
		{
			IsLocked = true;

			return this;
		}

		/// <summary>
		///     Check whether the object is a builder or not. (Hint: it is always a builder).
		/// </summary>
		public bool IsBuilder => true;

		/// <summary>
		///     Build the data pipeline. This can be done any time.
		///     A new reference is created every time, this method is called.
		/// </summary>
		/// <returns>The newly create <see cref="IDataPipeline" />.</returns>
		public abstract IDataPipeline Build();
	}
}