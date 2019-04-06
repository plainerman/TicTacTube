using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Executors.Events;
using TicTacTubeCore.Processors.Logical;
using TicTacTubeCore.Sources.Files;
using TicTacTubeTest.Sources.Files;

namespace TicTacTubeTest.Processors.Logical
{
	[TestClass]
	public class TestConditionalProcessor
	{
		[TestMethod]
		public void TestExecuteA()
		{
			TestEval(source => true);
			TestEval(source => false);
		}

		[TestMethod]
		public void TestExecuteB()
		{
			TestEval(source => true, true);
			TestEval(source => false, true);
		}

		[TestMethod]
		public void TestBadConstructor()
		{
			Assert.ThrowsException<ArgumentNullException>(() =>
				new ConditionalProcessor(null, new MockDataProcessor(), new MockDataProcessor()));
			Assert.ThrowsException<ArgumentNullException>(() =>
				new ConditionalProcessor(source => true, null, null));
			Assert.ThrowsException<ArgumentNullException>(() =>
				new ConditionalProcessor(source => true, null));
		}

		private static void TestEval(Func<IFileSource, bool> evalFunc, bool useSourceB = false)
		{
			var scheduler = new SimpleTestScheduler();
			int execCount = 0;

			scheduler.Scheduler.Executor.LifeCycleEvent += Executed;

			scheduler.Builder.Append(useSourceB
				? new ConditionalProcessor(evalFunc, null, new MockDataProcessor())
				: new ConditionalProcessor(evalFunc, new MockDataProcessor(), null));

			scheduler.Start();

			scheduler.ExecuteBlocking(new MockFileSource());

			Assert.AreEqual(1, execCount);

			void Executed(object sender, ExecutorLifeCycleEventArgs args)
			{
				if (args.EventType == ExecutorLifeCycleEventType.SourceExecutionFinished)
					execCount++;
			}
		}
	}
}