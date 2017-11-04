using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors;
using TicTacTubeCore.Processors.Filesystem;
using TicTacTubeCoreTest.Sources.Files;

namespace TicTacTubeCoreTest.Processors
{
	[TestClass]
	public class TestSourceMover
	{
		[TestMethod]
		public void TestMoveKeepFileName()
		{
			var scheduler = PrepareMove(out var source, out var sourceSize, out var destinatioPath);
			var destinationFile = Path.Combine(destinatioPath, source.FileInfo.Name);

			scheduler.Builder.Append(new SourceMover(destinatioPath, true));

			TestFileMovement(destinationFile, scheduler, source, sourceSize, destinatioPath);
		}

		[TestMethod]
		public void TestMove()
		{
			var scheduler = PrepareMove(out var source, out var sourceSize, out var destinatioPath);
			var destinationFile = Path.Combine(destinatioPath, "newFile");

			scheduler.Builder.Append(new SourceMover(destinationFile));

			TestFileMovement(destinationFile, scheduler, source, sourceSize, destinatioPath);
		}

		private static void TestFileMovement(string destinationFile, SimpleTestScheduler scheduler, TempFileSource source,
			long sourceSize, string destinatioPath)
		{
			Assert.AreEqual(false, File.Exists(destinationFile));

			scheduler.Execute(source);

			Assert.AreEqual(true, File.Exists(destinationFile));
			Assert.AreEqual(sourceSize, new FileInfo(destinationFile).Length);

			Directory.Delete(destinatioPath, true);
		}

		private static SimpleTestScheduler PrepareMove(out TempFileSource source, out long sourceSize,
			out string destinatioPath)
		{
			var scheduler = new SimpleTestScheduler();
			source = new TempFileSource();
			sourceSize = source.FileInfo.Length;

			destinatioPath = Path.Combine(Path.GetTempPath(), ".subfolder");
			return scheduler;
		}
	}
}