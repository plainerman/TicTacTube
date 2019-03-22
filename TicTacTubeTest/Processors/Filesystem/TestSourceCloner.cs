using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Filesystem;
using TicTacTubeCore.Sources.Files;
using TicTacTubeTest.Sources.Files;

namespace TicTacTubeTest.Processors.Filesystem
{
	[TestClass]
	public class TestSourceCloner
	{
		[TestMethod]
		public void TestSourceClone()
		{
			var scheduler = InitVars(out var source, out string newFolderPath, out string newFilePath);

			scheduler.Builder.Append(new SourceCloner(newFilePath, false));

			scheduler.Start();

			TestBasicClone(source, newFilePath, scheduler, newFolderPath);

			AppendDeleteAndExecute(scheduler, source);

			TestWorkOnOriginal(source, newFilePath, newFolderPath);
		}

		[TestMethod]
		public void TestSourceCloneWorkOnClone()
		{
			var scheduler = InitVars(out var source, out string newFolderPath, out string newFilePath);

			scheduler.Builder.Append(new SourceCloner(newFilePath, true));

			scheduler.Start();

			TestBasicClone(source, newFilePath, scheduler, newFolderPath);

			AppendDeleteAndExecute(scheduler, source);

			TestWorkOnClone(source, newFilePath, newFolderPath);
		}

		[TestMethod]
		public void TestSourceCloneKeepName()
		{
			var scheduler = InitVars(out var source, out string newFolderPath);
			string newFilePath = Path.Combine(newFolderPath, source.FileInfo.Name);

			scheduler.Builder.Append(new SourceCloner(newFolderPath, false, true));

			scheduler.Start();

			TestBasicClone(source, newFilePath, scheduler, newFolderPath);

			AppendDeleteAndExecute(scheduler, source);

			TestWorkOnOriginal(source, newFilePath, newFolderPath);
		}

		[TestMethod]
		public void TestSourceCloneWorkOnCloneKeepName()
		{
			var scheduler = InitVars(out var source, out string newFolderPath);
			string newFilePath = Path.Combine(newFolderPath, source.FileInfo.Name);

			scheduler.Builder.Append(new SourceCloner(newFolderPath, true, true));

			scheduler.Start();

			TestBasicClone(source, newFilePath, scheduler, newFolderPath);

			AppendDeleteAndExecute(scheduler, source);

			TestWorkOnClone(source, newFilePath, newFolderPath);
		}

		#region TestUtils

		private static void AppendDeleteAndExecute(SimpleTestScheduler scheduler, IFileSource source)
		{
			scheduler.Builder.Append(new SourceDeleter());
			scheduler.ExecuteBlocking(source);
		}

		#endregion TestUtils

		#region TestMethods

		private static void TestWorkOnOriginal(IFileSource source, string newFilePath, string newFolderPath)
		{
			Assert.AreEqual(false, File.Exists(source.FileInfo.FullName));
			Assert.AreEqual(true, File.Exists(newFilePath));

			Directory.Delete(newFolderPath, true);
		}

		private static void TestWorkOnClone(IFileSource source, string newFilePath, string newFolderPath)
		{
			Assert.AreEqual(true, File.Exists(source.FileInfo.FullName));
			Assert.AreEqual(false, File.Exists(newFilePath));

			Directory.Delete(newFolderPath, true);
		}

		private static void TestBasicClone(IFileSource source, string newFilePath, SimpleTestScheduler scheduler,
			string newFolderPath)
		{
			Assert.AreEqual(true, File.Exists(source.FileInfo.FullName));
			Assert.AreEqual(false, File.Exists(newFilePath));

			scheduler.ExecuteBlocking(source);

			Assert.AreEqual(true, File.Exists(source.FileInfo.FullName));
			Assert.AreEqual(true, File.Exists(newFilePath));

			Directory.Delete(newFolderPath, true);
		}

		#endregion TestMethods

		#region InitUtils

		private static SimpleTestScheduler InitVars(out IFileSource source, out string newFolderPath, out string newFilePath)
		{
			var scheduler = InitVars(out source, out newFolderPath);
			const string newFileName = ".tictactubetest";

			newFilePath = Path.Combine(newFolderPath, newFileName);

			return scheduler;
		}

		private static SimpleTestScheduler InitVars(out IFileSource source, out string newFolderPath)
		{
			var scheduler = new SimpleTestScheduler();
			source = new TempFileSource();

			newFolderPath = Path.Combine(Path.GetTempPath(), ".clone");

			return scheduler;
		}

		#endregion InitUtils
	}
}