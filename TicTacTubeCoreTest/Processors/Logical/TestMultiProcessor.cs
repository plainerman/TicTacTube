using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Processors.Logical;
using TicTacTubeCoreTest.Sources.Files;

namespace TicTacTubeCoreTest.Processors.Logical
{
	[TestClass]
	public class TestMultiProcessor
	{
		[TestMethod]
		public void TestExecute()
		{
			var scheduler = new SimpleTestScheduler();

			const int executorsCount = 5;
			var processors = new DebugProcessor[executorsCount];

			for (int i = 0; i < processors.Length; i++)
			{
				processors[i] = new DebugProcessor(new MockDataProcessor());
			}

			CheckExecutionCount(0, processors);

			scheduler.Builder.Append(new MultiProcessor(processors));
			scheduler.Scheduler.Start();

			scheduler.Execute(new MockFileSource());

			CheckExecutionCount(1, processors);

			scheduler.Execute(new MockFileSource());

			CheckExecutionCount(2, processors);
		}

		protected void CheckExecutionCount(int count, IEnumerable<DebugProcessor> processors)
		{
			foreach (var processor in processors)
			{
				Assert.AreEqual(count, processor.ExecutionCount);
			}
		}
	}
}
