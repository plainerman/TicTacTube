using System.Threading;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Schedulers
{
	/// <summary>
	/// A scheduler that polls a given method with a delay between. The pipeline will be executed from a different thread (local polling thread).
	/// </summary>
	public abstract class PollingBaseScheduler : BaseScheduler
	{
		/// <summary>
		/// The priority of the updates. The higher, the more frequent the updates will be.
		/// </summary>
		public ThreadPriority Priority { get; set; }

		/// <summary>
		/// The base amount for the delay in millis. For every <see cref="Priority"/>, this class waits this delay before the next poll.
		/// </summary>
		protected readonly int DelayInMillis;
		/// <summary>
		/// The thread that is used to execute the poll logic.
		/// </summary>
		protected Thread PollThread;

		/// <summary>
		/// Create a new polling scheduler.
		/// </summary>
		/// <param name="priority">The priority of the updates. The higher, the more frequent the updates will be.</param>
		/// <param name="delayInMillis">The base amount for the delay in millis. For every <paramref name="priority"/>, this class waits this delay before the next poll.</param>
		protected PollingBaseScheduler(ThreadPriority priority, int delayInMillis = 15_000)
		{
			DelayInMillis = delayInMillis;
			Priority = priority;
		}

		/// <inheritdoc />
		protected override void ExecuteStart()
		{
			PollThread = new Thread(Poll);
		}

		/// <summary>
		/// The actual poll logic.
		/// </summary>
		protected virtual void Poll()
		{
			while (IsRunning)
			{
				if (PollSource(out var sources))
				{
					// maybe poll takes really long 
					if (!IsRunning) break;
					foreach (var source in sources)
					{
						Execute(source);
					}
				}
				Thread.Sleep(DelayInMillis * (ThreadPriority.Highest - Priority + 1));
			}
		}

		/// <inheritdoc />
		protected override void OnStarted()
		{
			PollThread.Start();
		}

		/// <inheritdoc />
		protected override void ExecuteStop()
		{

		}

		/// <summary>
		/// Check whether a new source (or sources) could be polled.
		/// </summary>
		/// <param name="sources">The sources (if any have been found).</param>
		/// <returns><c>True</c>, if a source could be found - <c>false</c> otherwise.</returns>
		protected abstract bool PollSource(out IFileSource[] sources);
	}
}