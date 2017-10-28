using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Schedulers.Events;

namespace TicTacTubeCore.Schedulers
{
	public abstract class BaseScheduler : IScheduler
	{
		protected readonly List<IDataPipeline> InternalPipelines;

		protected BaseScheduler()
		{
			InternalPipelines = new List<IDataPipeline>();
			Pipelines = InternalPipelines.AsReadOnly();
		}

		public event EventHandler<SchedulerLifeCycleEventArgs> LifeCycleEvent;

		public bool IsRunning { get; protected set; }

		public ReadOnlyCollection<IDataPipeline> Pipelines { get; }

		public virtual void Add(IDataPipeline pipeline)
		{
			InternalPipelines.Add(pipeline);
		}

		public virtual void Add(IDataPipelineBuilder builder)
		{
			Add(builder.Build());
		}


		public virtual void Start()
		{
			ExecuteStart();
			IsRunning = true;
			ExecuteEvent(SchedulerLifeCycleEventType.Start);
		}

		public virtual void Stop()
		{
			ExecuteStop();
			IsRunning = false;
			ExecuteEvent(SchedulerLifeCycleEventType.Stop);
		}

		/// <summary>
		///     This method is called before setting the global running state to <c>true</c>.
		/// </summary>
		protected abstract void ExecuteStart();

		/// <summary>
		///     This method is called before setting the global running state to <c>false</c>.
		/// </summary>
		protected abstract void ExecuteStop();

		/// <summary>
		///     The method that will be called internally to execute the pipeline.
		/// </summary>
		protected virtual void Execute( /* TODO: args */)
		{
			// TODO: execute the complete pipeline
			//InternalPipelines.ForEach(p => p.Execute(...));
			ExecuteEvent(SchedulerLifeCycleEventType.Execute);
		}

		/// <summary>
		///     Execute a lifecycle event with given parameters.
		/// </summary>
		/// <param name="eventType">The type of the event.</param>
		protected virtual void ExecuteEvent(SchedulerLifeCycleEventType eventType)
		{
			LifeCycleEvent?.Invoke(this, new SchedulerLifeCycleEventArgs(IsRunning, eventType));
		}
	}
}