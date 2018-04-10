using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Pipelines.Exceptions;
using TicTacTubeTest.Processors;

namespace TicTacTubeTest.Pipelines
{
	[TestClass]
	public class TestDataPipelineBuilder
	{
		protected virtual IDataPipelineBuilder CreatePipelineBuilder() => new DataPipelineBuilder();

		[TestMethod]
		public void TestBuild()
		{
			var builder = CreatePipelineBuilder();
			builder.Append(new MockDataProcessor());

			Assert.IsInstanceOfType(builder.Build(), typeof(DataPipeline));
		}

		[TestMethod]
		public void TestAppend()
		{
			var builder = CreatePipelineBuilder();

			Assert.AreEqual(0, builder.Pipeline.Count);

			builder.Append(new MockDataProcessor());

			Assert.AreEqual(1, builder.Pipeline.Count);
		}

		[TestMethod]
		public void TestLock()
		{
			var builder = CreatePipelineBuilder();

			Assert.AreEqual(false, builder.IsLocked);

			builder.LockBuilder();

			Assert.AreEqual(true, builder.IsLocked);

			Assert.ThrowsException<PipelineStateException>(() => builder.Append(new MockDataProcessor()));
		}
	}
}