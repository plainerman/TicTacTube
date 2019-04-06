using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using log4net;
using TicTacTubeCore.Executors;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Schedulers.Events;
using TicTacTubeCore.Schedulers.Exceptions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Schedulers
{
	/// <inheritdoc />
	/// <summary>
	///     A scheduler that executes a data pipelineOrBuilder on some event.
	/// </summary>
	public abstract class BaseScheduler : IScheduler
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(BaseScheduler));

		/// <summary>
		///     Multiple pipelines that are executed on a certain condition / event.
		/// </summary>
		protected readonly List<IDataPipelineOrBuilder> InternalPipelines;

		/// <summary>
		///		This thread-safe collection stores file sources that have not been added to a IPipelineProcessor.
		/// </summary>
		protected ConcurrentDictionary<IFileSource, Predicate<IFileSource>> QueuedSources;

		/// <summary>
		/// The thread that will check all sources with a delay if they are available and adds sources that
		/// become available to the executor.
		/// </summary>
		private Thread _sourceUpdater;

		/// <summary>
		/// The semaphore used to force an update of the <see cref="_sourceUpdater"/>.
		/// </summary>
		private ManualResetEventSlim _sourceRequestedUpdate;

		/// <summary>
		/// The delay that will be waited before checking all queued sources again, if they have already become available.
		/// The unit is milliseconds. The default value is 1000 milliseconds.
		/// </summary>
		public int SourceConditionDelay { get; set; } = 1000;

		/// <summary>
		/// When the scheduler should stop (see <see cref="Stop"/>), it will retry the already stored sources
		/// every <see cref="SourceConditionDelay"/> milliseconds. For up to the specified number of times.
		/// If the sources do not become active with this number of tries, it will discard them.
		///
		/// If this number is negative, it will be tried indefinitely, possibly preventing the scheduler from stopping.
		/// </summary>
		public int StopRetryCount { get; set; } = -1;

		/// <summary>
		/// The actual variable that keeps count of <see cref="StopRetryCount"/>. 
		/// </summary>
		protected int CurrentStopRetryCount = 0;

		/// <inheritdoc />
		public event EventHandler<SchedulerLifeCycleEventArgs> LifeCycleEvent;

		/// <inheritdoc />
		public IExecutor Executor { get; }

		/// <inheritdoc />
		public bool IsRunning { get; protected set; }

		/// <inheritdoc />
		public bool Stopped { get; protected set; }

		/// <inheritdoc />
		public ReadOnlyCollection<IDataPipelineOrBuilder> Pipelines { get; }

		/// <summary>
		/// Create a base scheduler with a given executor.
		/// </summary>
		/// <param name="executor">The executor that will be used for processing the pipelines.
		/// If <code>null</code>, a single threaded executor will be used.</param>
		protected BaseScheduler(IExecutor executor)
		{
			Executor = executor ?? new Executor(1);
			InternalPipelines = new List<IDataPipelineOrBuilder>();
			Pipelines = InternalPipelines.AsReadOnly();
			QueuedSources = new ConcurrentDictionary<IFileSource, Predicate<IFileSource>>();
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
		///     The method that will be called internally to execute the pipelineOrBuilder. No condition will be added, so
		///		all sources are immediately accepted.
		///
		///		If the scheduler is not running, the source will not be added.
		/// </summary>
		/// <param name="fileSource">The fileSource with which the execute will be triggered.</param>
		protected virtual void Execute(IFileSource fileSource)
		{
			Execute(fileSource, s => true);
		}

		///  <summary>
		///		The method that will be called internally to execute the pipelineOrBuilder. It will be regularly checked, whether
		///		the condition evaluates to <code>true</code> - once it does, the source will be triggered to execute.
		///
		///		If the scheduler is not running, the source will not be added.
		///  </summary>
		///  <param name="fileSource">The fileSource with which the execute will be triggered.</param>
		///  <param name="waitCondition">
		///		A predicate determining when a given <paramref name="fileSource"/> is ready.
		///		If this method throws an exception, the source will be discarded.
		///  </param>
		protected virtual void Execute(IFileSource fileSource, Predicate<IFileSource> waitCondition)
		{
			if (!IsRunning) return;
			//TODO: this does not allow two reference identical file sources
			//TODO: it was not designed to support that - but should it?
			QueuedSources.TryAdd(fileSource, waitCondition);
			_sourceRequestedUpdate.Set();
		}

		/// <summary>
		/// The method that will be added to <see cref="_sourceUpdater"/>.
		/// While the scheduler is running (or there are sources left), it will check all sources if they become available.
		/// </summary>
		private void SourceUpdater_Thread()
		{
			while (IsRunning || !QueuedSources.IsEmpty)
			{
				if (!IsRunning) Log.DebugFormat("{0} is waiting for {1} source(s).", GetType().Name, QueuedSources.Count);

				if (!_sourceRequestedUpdate.Wait(SourceConditionDelay)) // wait for forced update, or else every second
					if (!IsRunning && StopRetryCount >= 0) CurrentStopRetryCount++;

				_sourceRequestedUpdate.Reset();

				// Try all queued sources and find those available.
				ProcessQueuedSources();

				if (!IsRunning && StopRetryCount >= 0 && CurrentStopRetryCount >= StopRetryCount) break;
			}

			// discard the remaining sources
			foreach (var source in QueuedSources.Keys)
			{
				ExecuteEvent(SchedulerLifeCycleEventType.SourceDiscarded, source);
			}
		}

		/// <summary>
		/// This method processes all queued sources. If a source becomes available it will be removed
		/// from the queued sources and added to the executor.
		/// </summary>
		protected virtual void ProcessQueuedSources()
		{
			foreach (var queuedSource in QueuedSources)
			{
				bool abort = false;
				try
				{
					if (!queuedSource.Value(queuedSource.Key)) continue; // skip if not ready
				}
				catch (Exception e)
				{
					Log.Debug(e);
					abort = true;
				}

				if (QueuedSources.TryRemove(queuedSource.Key, out _))
				{
					Log.InfoFormat("Source has become ready, executing {0} pipelineOrBuilder(s).",
						InternalPipelines.Count);

					ExecuteEvent(abort
						? SchedulerLifeCycleEventType.SourceDiscarded
						: SchedulerLifeCycleEventType.SourceReady, queuedSource.Key);

					if (!abort) Executor.Add(queuedSource.Key);
				}
			}
		}

		/// <summary>
		///     Execute a lifecycle event with given parameters.
		/// </summary>
		/// <param name="eventType">The type of the event.</param>
		/// <param name="source">The source that is processed by this event. May be <code>null</code>.</param>
		protected virtual void ExecuteEvent(SchedulerLifeCycleEventType eventType, IFileSource source = null)
		{
			LifeCycleEvent?.Invoke(this, new SchedulerLifeCycleEventArgs(IsRunning, source, eventType));
		}

		/// <inheritdoc />
		public virtual IDataPipelineOrBuilder Add(IDataPipelineOrBuilder pipelineOrBuilder)
		{
			lock (this)
			{
				if (IsRunning)
					throw new SchedulerException("The pipeline cannot be modified until the scheduler is stopped.");

				InternalPipelines.Add(pipelineOrBuilder);
				return pipelineOrBuilder;
			}
		}

		/// <inheritdoc />
		public virtual void Start()
		{
			lock (this)
			{
				if (Stopped)
					throw new SchedulerStateException(
						"The scheduler has already been stopped and cannot be restarted.");
				if (IsRunning) return;

				ExecuteStart();

				_sourceRequestedUpdate = new ManualResetEventSlim();
				_sourceUpdater = new Thread(SourceUpdater_Thread);

				Executor.Initialize(Pipelines);

				IsRunning = true;

				_sourceUpdater.Start();

				ExecuteEvent(SchedulerLifeCycleEventType.Start);
			}
		}

		/// <inheritdoc />
		/// <summary>
		///	This method forces to stop the scheduler. Once stopped, it cannot be restarted.
		/// </summary>
		public virtual void Stop()
		{
			lock (this)
			{
				if (!IsRunning) return;

				IsRunning = false;

				ExecuteStop();

				ExecuteEvent(SchedulerLifeCycleEventType.Stop);

				_sourceUpdater.Join();
				Stopped = true;
			}
		}

		/// <inheritdoc />
		public virtual void Join()
		{
			_sourceUpdater.Join();
			Executor.Stop();
		}
	}
}