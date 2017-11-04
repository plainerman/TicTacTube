using System;
using System.Collections.ObjectModel;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Schedulers.Events;

namespace TicTacTubeCore.Schedulers
{
	/// <summary>
	///     A scheduler that executes a data pipelineOrBuilder on some event.
	/// </summary>
	public interface IScheduler
	{
		/// <summary>
		///     Determine whether this scheduler is currently running.
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		///     The pipelines that will be executed.
		/// </summary>
		ReadOnlyCollection<IDataPipelineOrBuilder> Pipelines { get; }

		/// <summary>
		///     An event that is called whenever a lifecycle event has been called (e.g. start, pipelineOrBuilder executed, stopped
		///     ...)
		/// </summary>
		event EventHandler<SchedulerLifeCycleEventArgs> LifeCycleEvent;

		/// <summary>
		///     Add a new pipelineOrBuilder to the scheduler. This can either be the pipelineOrBuilder itself
		///     or a builder for a pipelineOrBuilder.
		/// </summary>
		/// <param name="pipeline">The pipelineOrBuilder that will be added.</param>
		/// <returns>The added data pipelineOrBuilder.</returns>
		IDataPipelineOrBuilder Add(IDataPipelineOrBuilder pipeline);

		/// <summary>
		///     Start the lifecycle of the scheduler.
		/// </summary>
		void Start();

		/// <summary>
		///     Force the scheduler to stop updating.
		/// </summary>
		void Stop();
	}
}