using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Logical;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCoreTest.Sources.Files;

namespace TicTacTubeCoreTest.Processors.Logical
{
	[TestClass]
	public class TestConditionalProcessor
	{
		[TestMethod]
		public void TestExecuteA()
		{
			TestEval(source => true, false, 1);
			TestEval(source => false, false, 0);
		}

		[TestMethod]
		public void TestExecuteB()
		{
			TestEval(source => true, true, 0);
			TestEval(source => false, true, 1);
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

		private static void TestEval(Func<IFileSource, bool> evalFunc, bool useSourceB, int desiredExecutionCount)
		{
			var scheduler = new SimpleTestScheduler();

			var dataProcessor = new MockDataProcessor();

			scheduler.Builder.Append(useSourceB
				? new ConditionalProcessor(evalFunc, null, dataProcessor)
				: new ConditionalProcessor(evalFunc, dataProcessor, null));

			scheduler.Scheduler.Start();

			scheduler.Execute(new MockFileSource());

			Assert.AreEqual(desiredExecutionCount, dataProcessor.ExecutionCount);
		}
	}
}