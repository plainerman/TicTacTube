using System;
using System.Collections.Generic;
using TicTacTubeCore.Executors.Events;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Executors
{
	/// <summary>
	/// The interface defining how an executor has to be implemented. Every scheduler has an executor that actually executes the pipeline.
	/// In here it can be controlled how processors are executed (i.e. single- / multi-threaded, allowing a file source to only be processed once, ...)
	/// </summary>
	public interface IExecutor
	{
		/// <summary>
		/// Whether the current executor is initialized and up and running.
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		///     An event that is called whenever an important action occurs.
		/// </summary>
		event EventHandler<ExecutorLifeCycleEventArgs> LifeCycleEvent;

		/// <summary>
		/// Tells the executor to initialize everything required. Depending on the implementation, multiple calls may be ignored.
		/// </summary>
		/// <param name="pipeline">The pipeline this executor will be initialized with.</param>
		void Initialize(IEnumerable<IDataPipelineOrBuilder> pipeline);

		/// <summary>
		/// Add a specified <paramref name="fileSource"/> to the executor. It may not be added.
		/// </summary>
		/// <param name="fileSource">The source that may be added.</param>
		/// <returns><code>true</code>, if <paramref name="fileSource"/> has been added, <code>false</code> otherwise.</returns>
		bool Add(IFileSource fileSource);

		/// <summary>
		/// Wait for the executor to finish up all pipelines (i.e. finish all added sources).
		/// After it has been stopped, the executor cannot be reused.
		/// </summary>
		void Stop();
	}
}