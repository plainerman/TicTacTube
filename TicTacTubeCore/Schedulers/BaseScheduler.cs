﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using log4net;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Schedulers.Events;
using TicTacTubeCore.Schedulers.Exceptions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Schedulers
{
	/// <summary>
	///     A scheduler that executes a data pipelineOrBuilder on some event.
	/// </summary>
	public abstract class BaseScheduler : IScheduler
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(BaseScheduler));

		/// <summary>
		///     Multiple pipelines that are executed on a certion condition / event.
		/// </summary>
		protected readonly List<IDataPipelineOrBuilder> InternalPipelines;

		/// <summary>
		///     The default constructor.
		/// </summary>
		protected BaseScheduler()
		{
			InternalPipelines = new List<IDataPipelineOrBuilder>();
			Pipelines = InternalPipelines.AsReadOnly();
		}

		/// <inheritdoc />
		public event EventHandler<SchedulerLifeCycleEventArgs> LifeCycleEvent;

		/// <inheritdoc />
		public bool IsRunning { get; protected set; }

		/// <inheritdoc />
		public ReadOnlyCollection<IDataPipelineOrBuilder> Pipelines { get; }

		/// <inheritdoc />
		public virtual IDataPipelineOrBuilder Add(IDataPipelineOrBuilder pipelineOrBuilder)
		{
			InternalPipelines.Add(pipelineOrBuilder);
			return pipelineOrBuilder;
		}


		/// <inheritdoc />
		public virtual void Start()
		{
			ExecuteStart();
			IsRunning = true;
			OnStarted();
			ExecuteEvent(SchedulerLifeCycleEventType.Start);
		}

		/// <inheritdoc />
		public virtual void Stop()
		{
			ExecuteStop();
			IsRunning = false;
			OnStopped();
			ExecuteEvent(SchedulerLifeCycleEventType.Stop);
		}

		/// <summary>
		/// This method will be called after <see cref="ExecuteStart"/>, once <see cref="IsRunning"/> is <c>true</c>. (<see cref="LifeCycleEvent"/> not yet executed).
		/// </summary>
		protected virtual void OnStarted() { }

		/// <summary>
		/// This method will be called after <see cref="ExecuteStop"/>, once <see cref="IsRunning"/> is <c>false</c>. (<see cref="LifeCycleEvent"/> not yet executed).
		/// </summary>
		protected virtual void OnStopped() { }

		/// <summary>
		///     This method is called before setting the global running state to <c>true</c>.
		/// </summary>
		protected abstract void ExecuteStart();

		/// <summary>
		///     This method is called before setting the global running state to <c>false</c>.
		/// </summary>
		protected abstract void ExecuteStop();

		/// <summary>
		///     The method that will be called internally to execute the pipelineOrBuilder.
		/// </summary>
		/// <param name="fileSource">The filesource with which the execute will be triggered.</param>
		protected virtual void Execute(IFileSource fileSource)
		{
			if (!IsRunning) throw new SchedulerStateException("The scheduler is not running.");

			Log.Info($"Scheduler has been triggered, executing {InternalPipelines.Count} pipelineOrBuilder(s) for {fileSource?.FullFileName}.");
			InternalPipelines.ForEach(p => p.Build().Execute(fileSource));
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