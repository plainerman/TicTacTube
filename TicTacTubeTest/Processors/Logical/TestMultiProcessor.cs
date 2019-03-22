using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Logical;
using TicTacTubeCore.Sources.Files;
using TicTacTubeTest.Sources.Files;

namespace TicTacTubeTest.Processors.Logical
{
	[TestClass]
	public class TestMultiProcessor
	{
		[TestMethod]
		public void TestConstructor()
		{
			var processor = new MultiProcessor(new LambdaProcessor(source => new FileSource(source.FileName + "1")));
			Assert.AreEqual("test1", processor.Build().Execute(new FileSource("test")).FileName);

			processor = new MultiProcessor(
				new LambdaProcessor(source => new FileSource(source.FileName + "1")),
				new LambdaProcessor(source => new FileSource(source.FileName + "2"))
				);
			Assert.AreEqual("test12", processor.Build().Execute(new FileSource("test")).FileName);

			processor = new MultiProcessor(
				new LambdaProcessor(source => new FileSource(source.FileName + "1")),
				new LambdaProcessor(source => new FileSource(source.FileName + "2")),
				new LambdaProcessor(source => new FileSource(source.FileName + "3"))
			);
			Assert.AreEqual("test123", processor.Build().Execute(new FileSource("test")).FileName);
		}

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
			scheduler.Start();

			scheduler.ExecuteBlocking(new MockFileSource());

			CheckExecutionCount(1, processors);

			scheduler.ExecuteBlocking(new MockFileSource());

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