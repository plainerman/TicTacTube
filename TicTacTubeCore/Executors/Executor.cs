using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;
using TicTacTubeCore.Executors.Events;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Sources.Files.Comparer;

namespace TicTacTubeCore.Executors
{
	/// <inheritdoc />
	/// <summary>
	/// An executor that can execute pipelines with a specified number of threads.
	/// When adding a file source, that has already been added by calling the <see cref="Add"/>-function,
	/// the second will wait for the first to be fully executed.
	/// </summary>
	public class Executor : IExecutor
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(Executor));

		/// <inheritdoc />
		public event EventHandler<ExecutorLifeCycleEventArgs> LifeCycleEvent;

		/// <summary>
		/// When this boolean is set, the executor will stop if an exception occurs during execution of the pipeline.
		/// Pending sources will not be aborted, simply <see cref="Stop"/> will be called automatically.
		/// The default value is <code>false</code>.
		///
		/// If an error occurs inside a pipeline, the complete pipeline will always be skipped.
		/// </summary>
		public bool DieOnException { get; set; } = false;

		/// <summary>
		/// When this boolean is set, the executor will skip all other pipelines that should be executed for a given source.
		/// The default value is <code>true</code>, to ignore exceptions set this to <code>true</code>.
		///
		/// If an error occurs inside a pipeline, the complete pipeline will always be skipped.
		/// </summary>
		public bool AbortPipelineOnError { get; set; } = true;

		/// <summary>
		/// A semaphore counting the queued elements (0 means no queued elements).
		/// </summary>
		private readonly SemaphoreSlim _queuedSemaphore;

		/// <summary>
		/// In this collection all pending file sources are stored. Threads then pick an element and remove it.
		/// </summary>
		protected readonly IProducerConsumerCollection<IFileSource> PendingFileSources;

		/// <summary>
		/// The file sources that are currently processed.
		/// </summary>
		protected readonly IList<IFileSource> ActiveFileSources;

		/// <summary>
		/// In this collection, all conflicting file sources are stored. Conflicted file sources, are file sources
		/// that are pending but locked due to a thread currently processing an identical source.
		/// If two sources are identical is determined by <see cref="FileSourceComparer"/>.
		/// </summary>
		protected readonly IList<IFileSource> ConflictingFileSources;

		/// <summary>
		/// The pipeline that will be executed by this executor. Is <code>null</code>,
		/// until <see cref="Initialize"/> is called.
		/// </summary>
		protected IEnumerable<IDataPipelineOrBuilder> Pipeline { get; set; }

		/// <inheritdoc />
		public bool IsRunning { get; protected set; }

		/// <summary>
		/// This boolean determines whether the executor has already been initialized or not.
		/// Per default, once set to <code>true</code> it will never become false.
		/// </summary>
		protected bool Initialized { get; set; }

		/// <summary>
		/// The array containing the executor threads. Once <see cref="Initialize"/> is called, the threads will be created and started.
		/// </summary>
		protected readonly Thread[] Executors;

		/// <summary>
		/// Whether this executor has already been stopped or not.
		/// </summary>
		private bool _fullyStopped;

		/// <summary>
		/// This comparer is used to ensure that not two identical file sources are started to process.
		/// </summary>
		protected IEqualityComparer<IFileSource> FileSourceComparer { get; }

		/// <inheritdoc />
		/// <summary>
		/// Create a new executor with a given number of threads. To compare two file sources, the
		/// full file names (i.e. full path) will be compared with <see cref="T:TicTacTubeCore.Sources.Files.Comparer.NameFileSourceComparer" />.
		/// </summary>
		/// <param name="threadCount">The number of threads that will process in parallel.
		/// This number has to be greater than zero.</param>
		public Executor(int threadCount) : this(threadCount, new NameFileSourceComparer())
		{
		}

		/// <summary>
		/// Create a new executor with a given number of threads.
		/// </summary>
		/// <param name="threadCount">The number of threads that will process in parallel.
		/// This number has to be greater than zero.</param>
		/// <param name="fileSourceComparer">This interface compares two file source. Two sources that are the same cannot be executed simultaneously.</param>
		public Executor(int threadCount, IEqualityComparer<IFileSource> fileSourceComparer)
		{
			if (threadCount <= 0) throw new ArgumentOutOfRangeException(nameof(threadCount));

			_queuedSemaphore = new SemaphoreSlim(0);
			PendingFileSources = new ConcurrentBag<IFileSource>();
			ActiveFileSources = new List<IFileSource>();
			ConflictingFileSources = new List<IFileSource>();

			Executors = new Thread[threadCount];

			FileSourceComparer = fileSourceComparer ?? throw new ArgumentNullException(nameof(fileSourceComparer));
		}

		/// <inheritdoc />
		/// <summary>
		///		This method assigns the given pipeline once (i.e. cannot be changed afterwards), and creates and starts the threads.
		///		Calling this method a second time will be ignored.
		/// </summary>
		public void Initialize(IEnumerable<IDataPipelineOrBuilder> pipeline)
		{
			lock (this)
			{
				if (Initialized) return;

				Pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));

				IsRunning = true;

				for (int i = 0; i < Executors.Length; i++)
				{
					Executors[i] = new Thread(PipelineExecution_Thread);
					Executors[i].Start();
				}

				Initialized = true;
			}

			LifeCycleEvent?.Invoke(this, new ExecutorLifeCycleEventArgs(ExecutorLifeCycleEventType.Initialize));
		}

		/// <summary>
		/// The method that will actually execute the logic of the executor.
		/// It is responsible for the correct running condition (i.e. when it should be stopped), taking sources, processing them,
		/// and activating the corresponding events for <see cref="LifeCycleEvent"/>.
		/// </summary>
		protected virtual void PipelineExecution_Thread()
		{
			while (IsRunning || PendingFileSources.Count > 0)
			{
				_queuedSemaphore.Wait();

				IFileSource source;
				// when forcing the thread to stop, we release the semaphore without 
				// actually adding an element, causing it to continue and break the loop
				lock (ActiveFileSources)
				{
					// ensure that the element is either active or pending
					if (!PendingFileSources.TryTake(out source)) continue;
					ActiveFileSources.Add(source);
				}

				ProcessSource(source);

				lock (ActiveFileSources)
				{
					if (ActiveFileSources.Remove(source))
					{
						var conflictingSource =
							ConflictingFileSources.FirstOrDefault(s => FileSourceComparer.Equals(s, source));

						if (conflictingSource != null && PendingFileSources.TryAdd(conflictingSource))
						{
							ConflictingFileSources.Remove(conflictingSource);
							_queuedSemaphore.Release();
						}
					}
				}
			}
		}

		/// <summary>
		/// This method actually processes the sources, invokes all required events, and handles errors.
		/// </summary>
		/// <param name="source">The source that will be executed.</param>
		protected virtual void ProcessSource(IFileSource source)
		{
			LifeCycleEvent?.Invoke(this,
				new ExecutorLifeCycleEventArgs(ExecutorLifeCycleEventType.SourceExecutionStart, source));

			Log.DebugFormat("Starting to process {0}.", source?.FileInfo);

			bool error = false;

			foreach (var p in Pipeline)
			{
				try
				{
					p.Build().Execute(source);
				}
				catch (Exception e)
				{
					error = true;
					IsRunning = !DieOnException; // prevent new sources from being added (if DieOnException)

					LifeCycleEvent?.Invoke(this, new ExecutorLifeCycleEventArgs(p, source, e));
					Log.WarnFormat("Source {0} has thrown an exception.", source?.FileInfo);
					if (AbortPipelineOnError) break;
				}
			}

			LifeCycleEvent?.Invoke(this,
				new ExecutorLifeCycleEventArgs(ExecutorLifeCycleEventType.SourceExecutionFinished, source));

			if (error && DieOnException)
			{
				new Thread(Stop).Start(); // call stop from another thread, so that it won't cause a deadlock.
			}
		}

		/// <summary>
		/// If the given file source is conflicting, it will be added to <see cref="ConflictingFileSources"/>.
		/// Conflicted file sources are file sources that are pending but locked due to a thread currently
		/// processing an identical source.
		/// </summary>
		/// <param name="source">The source that will be checked if it is conflicting and (maybe) added.</param>
		/// <returns><code>true</code> if the file source is currently active or already pending (i.e. conflicting);
		/// <code>false</code> otherwise.</returns>
		protected virtual bool TryAddConflicting(IFileSource source)
		{
			lock (ActiveFileSources)
			{
				if (PendingFileSources.Any(s => FileSourceComparer.Equals(s, source)) ||
				    ActiveFileSources.Any(s => FileSourceComparer.Equals(s, source)))
				{
					ConflictingFileSources.Add(source);
					return true;
				}
			}

			return false;
		}

		//TODO: format and fix imports

		/// <inheritdoc />
		public bool Add(IFileSource fileSource)
		{
			if (fileSource == null) throw new ArgumentNullException(nameof(fileSource));

			lock (this)
			{
				if (!IsRunning) return false;

				if (!TryAddConflicting(fileSource))
				{
					if (!PendingFileSources.TryAdd(fileSource)) return false;

					_queuedSemaphore.Release();
				}
			}

			LifeCycleEvent?.Invoke(this,
				new ExecutorLifeCycleEventArgs(ExecutorLifeCycleEventType.SourceAdded, fileSource));

			return true;
		}

		/// <inheritdoc />
		public void Stop()
		{
			lock (this)
			{
				if (_fullyStopped) return;

				IsRunning = false;

				// see PipelineExecution_Thread, basically we release for every thread
				// so every thread is able to stop
				// when stopping, every thread consumes exactly one semaphore
				_queuedSemaphore.Release(Executors.Length);

				foreach (var executor in Executors)
				{
					executor.Join();
				}

				_fullyStopped = true;
			}

			LifeCycleEvent?.Invoke(this, new ExecutorLifeCycleEventArgs(ExecutorLifeCycleEventType.Stop));
		}
	}
}