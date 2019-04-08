using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Filesystem;
using TicTacTubeCore.Sources.Files;
using TicTacTubeTest.Sources.Files;

namespace TicTacTubeTest.Processors.Filesystem
{
	[TestClass]
	public class TestSourceMover
	{
		[TestMethod]
		public void TestMoveKeepFileName()
		{
			var scheduler = PrepareMove(out var source, out long sourceSize, out string destinationPath);
			string destinationFile = Path.Combine(destinationPath, source.FileInfo.Name);

			scheduler.Builder.Append(new SourceMover(destinationPath, true, true));
			scheduler.Start();

			TestFileMovement(destinationFile, scheduler, source, sourceSize, destinationPath);
		}

		[TestMethod]
		public void TestMove()
		{
			var scheduler = PrepareMove(out var source, out long sourceSize, out string destinationPath);
			string destinationFile = Path.Combine(destinationPath, "newFile");

			scheduler.Builder.Append(new SourceMover(destinationFile));
			scheduler.Start();

			TestFileMovement(destinationFile, scheduler, source, sourceSize, destinationPath);
		}

		[TestMethod]
		public void TestMoveWithPathFunction()
		{
			var scheduler = PrepareMove(out var source, out long sourceSize, out string destinationPath);
			destinationPath = Path.Combine(destinationPath, ".nestedSubFolder");
			string destinationFile = Path.Combine(destinationPath, "yepCustomEval");

			scheduler.Builder.Append(new SourceMover(Path.Combine(destinationPath, destinationFile)));
			scheduler.Start();

			TestFileMovement(destinationFile, scheduler, source, sourceSize, destinationPath);
		}

		private static void TestFileMovement(string destinationFile, SimpleTestScheduler scheduler, IFileSource source,
			long sourceSize, string destinationPath)
		{
			Assert.AreEqual(false, File.Exists(destinationFile));

			scheduler.ExecuteBlocking(source);

			Assert.IsTrue(File.Exists(destinationFile));
			Assert.AreEqual(sourceSize, new FileInfo(destinationFile).Length);

			Directory.Delete(destinationPath, true);
		}

		private static SimpleTestScheduler PrepareMove(out TempFileSource source, out long sourceSize,
			out string destinationPath)
		{
			var scheduler = new SimpleTestScheduler();
			source = new TempFileSource();
			sourceSize = source.FileInfo.Length;

			destinationPath = Path.Combine(Path.GetTempPath(), ".subfolder");
			return scheduler;
		}
	}
}