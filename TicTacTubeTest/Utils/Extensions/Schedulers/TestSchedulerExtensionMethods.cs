using System;
using System.Reflection;
using System.Threading;
using TicTacTubeCore.Executors.Events;
using TicTacTubeCore.Schedulers;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeTest.Utils.Extensions.Schedulers
{
	public static class TestSchedulerExtensionMethods
	{
		public static void ExecuteBlocking(this BaseScheduler scheduler, IFileSource fileSource, int timeout = 5000)
		{
			var resetEvent = new ManualResetEventSlim();
			scheduler.Executor.LifeCycleEvent += Unlock;

			var methodInfo = typeof(BaseScheduler).GetMethod("Execute", BindingFlags.NonPublic | BindingFlags.Instance,
				                 null, new[] { typeof(IFileSource) }, null)
			                 ?? throw new MissingMethodException("Execute could not be found");

			methodInfo.Invoke(scheduler, new object[] { fileSource });

			if (resetEvent.Wait(timeout))
			{
				scheduler.Executor.LifeCycleEvent -= Unlock;
			}
			else
			{
				throw new TimeoutException("The file source has not been processed in the defined amount of time.");
			}

			void Unlock(object sender, ExecutorLifeCycleEventArgs args)
			{
				if (args.EventType == ExecutorLifeCycleEventType.SourceExecutionFinished &&
				    ReferenceEquals(args.FileSource, fileSource))
				{
					resetEvent.Set();
				}
			}
		}
	}
}