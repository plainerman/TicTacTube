using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Executors;
using TicTacTubeCore.Executors.Events;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Processors.Logical;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Sources.Files.Comparer;
using TicTacTubeTest.Sources.Files;
using TicTacTubeTest.Utils;
using TicTacTubeTest.Utils.Extensions.Executors;

namespace TicTacTubeTest.Executors
{
	[TestClass]
	public class ExecutorTest
	{
		[TestMethod]
		public void TestBasicExecution()
		{
			var executor = new Executor(1);
			var pipeline = new DataPipelineBuilder();

			pipeline.Append(new LambdaProcessor(s => { }));

			executor.Initialize(new[] { pipeline });

			executor.ExecuteBlocking(new MockFileSource(), true);
			executor.ExecuteBlocking(new MockFileSource(), true);

			executor.Stop();
		}

		[TestMethod]
		public void TestNoDieNoAbort()
		{
			var executor = new Executor(1) { AbortPipelineOnError = false, DieOnException = false };
			var collector = new EventCollector<ExecutorLifeCycleEventArgs>();

			CreatePipelineWithException(executor);

			executor.LifeCycleEvent += collector.Collect;

			executor.ExecuteBlocking(new MockFileSource(), true);

			AnalyzeCollector(collector, out int _, out int failedCount);

			// both pipelines are executed, and both fail
			Assert.AreEqual(2, failedCount);

			// due to no die, new file sources can be added
			executor.ExecuteBlocking(new MockFileSource(), true);

			AnalyzeCollector(collector, out int _, out failedCount);
			// with a second execution, two more should have failed 
			Assert.AreEqual(4, failedCount);

			executor.Stop();
			executor.LifeCycleEvent -= collector.Collect;
		}

		[TestMethod]
		public void TestDieAndAbort()
		{
			var executor = new Executor(1) { AbortPipelineOnError = true, DieOnException = true };
			var collector = new EventCollector<ExecutorLifeCycleEventArgs>();

			CreatePipelineWithException(executor);

			executor.LifeCycleEvent += collector.Collect;

			executor.ExecuteBlocking(new MockFileSource(), true);

			AnalyzeCollector(collector, out int _, out int failedCount);

			// after the first pipeline, the second (which also throws an exception) should not be executed
			Assert.AreEqual(1, failedCount);

			// due to die, new sources are not accepted anymore
			Assert.IsFalse(executor.Add(new MockFileSource()));

			Assert.IsTrue(collector.WaitFor(arg => arg.EventType == ExecutorLifeCycleEventType.Stop, 5000),
				"The executor did not stop in time (it should have died).");

			executor.LifeCycleEvent -= collector.Collect;
		}

		[TestMethod]
		public void TestMultiThreadedExecution()
		{
			var executor = new Executor(4, new ReferenceFileSourceComparer());
			var collector = new EventCollector<ExecutorLifeCycleEventArgs>(e => e.FileSource != null);

			CreateAndExecuteSimplePipeline(executor, collector, 8);

			Assert.IsTrue(HasConcurrency(collector), "It could not be observed, that two threads run concurrently. " +
			                                         "It is very unlikely, that this was just a coincidence.");
		}

		[TestMethod]
		public void TestDelayConflictingSources()
		{
			var executor = new Executor(3, new AlwaysEqualFileSourceComparer());
			var collector = new EventCollector<ExecutorLifeCycleEventArgs>(e => e.FileSource != null);

			CreateAndExecuteSimplePipeline(executor, collector, 4);

			Assert.AreEqual(4,
				collector.Events.Count(e => e.EventType == ExecutorLifeCycleEventType.SourceExecutionFinished),
				"Not all file sources have been execute successfully.");

			Assert.IsFalse(HasConcurrency(collector),
				"Two threads executed simultaneously that were not allowed to (conflicting file sources).");
		}

		private static void CreatePipelineWithException(IExecutor executor)
		{
			var pipelineA = new DataPipelineBuilder();
			var pipelineB = new DataPipelineBuilder();

			pipelineA.Append(new LambdaProcessor(s => throw new Exception("This exception is expected.")));
			pipelineA.Append(new LambdaProcessor(s => { }));

			pipelineB.Append(new LambdaProcessor(s => throw new Exception("This exception is expected.")));

			executor.Initialize(new[] { pipelineA, pipelineB });
		}

		private static void CreateAndExecuteSimplePipeline(IExecutor executor,
			EventCollector<ExecutorLifeCycleEventArgs> collector, int count, int sleep = 125)
		{
			var pipeline = new DataPipelineBuilder();

			pipeline.Append(new LambdaProcessor(s => Thread.Sleep(sleep)));

			executor.Initialize(new[] { pipeline });

			executor.LifeCycleEvent += collector.Collect;

			for (int i = 0; i < count; i++)
			{
				executor.Execute(new MockFileSource());
			}

			executor.Stop();

			executor.LifeCycleEvent -= collector.Collect;
		}

		private static bool HasConcurrency(EventCollector<ExecutorLifeCycleEventArgs> collector)
		{
			int runningSimultaneously = 0;
			foreach (var collectedEvent in collector.Events)
			{
				var currentEvent = collectedEvent.EventType;
				switch (currentEvent)
				{
					case ExecutorLifeCycleEventType.SourceExecutionStart:
						runningSimultaneously++;
						break;
					case ExecutorLifeCycleEventType.SourceExecutionFailed:
					case ExecutorLifeCycleEventType.SourceExecutionFinished:
						runningSimultaneously--;
						break;
				}

				if (runningSimultaneously > 1) return true;
			}

			return false;
		}

		private static void AnalyzeCollector(EventCollector<ExecutorLifeCycleEventArgs> collector,
			out int successCount, out int failedCount)
		{
			successCount = 0;
			failedCount = 0;
			foreach (var curArg in collector.Events)
			{
				if (curArg.EventType == ExecutorLifeCycleEventType.SourceExecutionFinished)
				{
					successCount++;
				}
				else if (curArg.EventType == ExecutorLifeCycleEventType.SourceExecutionFailed)
				{
					failedCount++;
				}
			}

			successCount -= failedCount;
		}

		private class AlwaysEqualFileSourceComparer : IEqualityComparer<IFileSource>
		{
			public bool Equals(IFileSource x, IFileSource y) => true;

			public int GetHashCode(IFileSource obj) => 0;
		}
	}
}