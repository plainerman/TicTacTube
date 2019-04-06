using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Schedulers;
using TicTacTubeTest.Processors;

namespace TicTacTubeTest.Schedulers
{
	[TestClass]
	public class FileSystemSchedulerTest
	{
		public void TestDirectoryWatch()
		{
			var scheduler = CreateScheduler(out string path);
			var dataProcessor = new MockDataProcessor();

			scheduler.Add(new DataPipelineBuilder().Append(dataProcessor));

			scheduler.Start();

			Assert.AreEqual(0, dataProcessor.ExecutionCount);
			Assert.IsFalse(File.Exists(Path.Combine(path, "tmp.txt")));
			File.WriteAllText(Path.Combine(path, "tmp.txt"), "");

			Assert.IsFalse(File.Exists(Path.Combine(path, "tmp2.txt")));
			File.Move(Path.Combine(path, "tmp.txt"), Path.Combine(path, "tmp2.txt"));

			Assert.IsTrue(File.Exists(Path.Combine(path, "tmp2.txt")));
			File.Delete(Path.Combine(path, "tmp2.txt"));

			scheduler.Stop();
			scheduler.Join();
			DisposeScheduler(scheduler);

			Assert.AreEqual(3, dataProcessor.ExecutionCount);
		}

		private FileSystemScheduler CreateScheduler(out string path)
		{
			path = Path.GetTempFileName();
			File.Delete(path);
			return new FileSystemScheduler(path);
		}

		private static void DisposeScheduler(FileSystemScheduler scheduler)
		{
			Directory.Delete(scheduler.Path, true);
		}
	}
}