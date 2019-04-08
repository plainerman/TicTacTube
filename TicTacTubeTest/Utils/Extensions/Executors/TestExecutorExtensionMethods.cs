using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Executors;
using TicTacTubeCore.Executors.Events;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeTest.Utils.Extensions.Executors
{
	public static class TestExecutorExtensionMethods
	{
		public static void Execute(this IExecutor executor, IFileSource fileSource)
		{
			Assert.IsTrue(executor.Add(fileSource), "The file source could not be added.");
		}

		public static void ExecuteBlocking(this IExecutor executor, IFileSource fileSource, bool ignoreFails = false,
			int timeout = 5000)
		{
			var resetEvent = new ManualResetEventSlim();
			executor.LifeCycleEvent += Unlock;

			try
			{
				executor.Execute(fileSource);
				if (!resetEvent.Wait(timeout))
				{
					throw new TimeoutException("The file source has not been processed in the defined amount of time.");
				}
			}
			finally
			{
				executor.LifeCycleEvent -= Unlock;
			}

			void Unlock(object sender, ExecutorLifeCycleEventArgs e)
			{
				if (!ReferenceEquals(e.FileSource, fileSource)) return;

				if (e.EventType == ExecutorLifeCycleEventType.SourceExecutionFinished)
				{
					resetEvent.Set();
				}
				else if (!ignoreFails && e.EventType == ExecutorLifeCycleEventType.SourceExecutionFailed)
				{
					throw new Exception("The execution of the file source failed.");
				}
			}
		}
	}
}