using System;
using System.Collections.ObjectModel;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Schedulers.Events;

namespace TicTacTubeCore.Schedulers
{
	/// <summary>
	///     A scheduler that executes a data pipeline on some event.
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
		ReadOnlyCollection<IDataPipeline> Pipelines { get; }

		/// <summary>
		///     The event that will be called on every lifecycle event.
		/// </summary>
		event EventHandler<SchedulerLifeCycleEventArgs> LifeCycleEvent;

		/// <summary>
		///     Add a new pipeline to the scheduler.
		/// </summary>
		/// <param name="pipeline">The pipeline that will be added.</param>
		void Add(IDataPipeline pipeline);

		/// <summary>
		///     Add a new pipeline to the scheduler (it will be built with the builder).
		/// </summary>
		/// <param name="builder">The builder that is used to create a pipeline that will be added.</param>
		void Add(IDataPipelineBuilder builder);

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