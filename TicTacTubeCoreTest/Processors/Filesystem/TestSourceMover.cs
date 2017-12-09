using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Filesystem;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCoreTest.Sources.Files;

namespace TicTacTubeCoreTest.Processors.Filesystem
{
	[TestClass]
	public class TestSourceMover
	{
		[TestMethod]
		public void TestMoveKeepFileName()
		{
			var scheduler = PrepareMove(out var source, out long sourceSize, out string destinatioPath);
			string destinationFile = Path.Combine(destinatioPath, source.FileInfo.Name);

			scheduler.Builder.Append(new SourceMover(destinatioPath, true, true));

			TestFileMovement(destinationFile, scheduler, source, sourceSize, destinatioPath);
		}

		[TestMethod]
		public void TestMove()
		{
			var scheduler = PrepareMove(out var source, out long sourceSize, out string destinatioPath);
			string destinationFile = Path.Combine(destinatioPath, "newFile");

			scheduler.Builder.Append(new SourceMover(destinationFile));

			TestFileMovement(destinationFile, scheduler, source, sourceSize, destinatioPath);
		}

		[TestMethod]
		public void TestMoveWithPathFunction()
		{
			var scheduler = PrepareMove(out var source, out var sourceSize, out var destinatioPath);
			destinatioPath = Path.Combine(destinatioPath, ".nestedSubFolder");
			string destinationFile = Path.Combine(destinatioPath, "yepCustomEval");

			scheduler.Builder.Append(new SourceMover(Path.Combine(destinatioPath, destinationFile)));

			TestFileMovement(destinationFile, scheduler, source, sourceSize, destinatioPath);
		}

		private static void TestFileMovement(string destinationFile, SimpleTestScheduler scheduler, IFileSource source,
			long sourceSize, string destinatioPath)
		{
			Assert.AreEqual(false, File.Exists(destinationFile));

			scheduler.Execute(source);

			Assert.IsTrue(File.Exists(destinationFile));
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