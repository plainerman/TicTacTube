using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Filesystem;
using TicTacTubeTest.Sources.Files;

namespace TicTacTubeTest.Processors.Filesystem
{
	[TestClass]
	public class TestSourceRenamer
	{
		[DataTestMethod]
		[DataRow(null, @"newFile.dat", null, DisplayName = "no subfolder")]
		[DataRow(@".subFolder-ticTac", @"newFile.dat2", null, DisplayName = "one subfolder")]
		[DataRow(@".subFolder-ticTac", @"newFile.dat2", "yes/multiple/subfolders/also/work", DisplayName =
			"multiple subfolders")]
		public void TestRename(string directory, string newFileName, string additionalFolders)
		{
			var scheduler = new SimpleTestScheduler();
			var source = new TempFileSource();
			long sourceSize = source.FileInfo.Length;

			string destinationPath = newFileName;
			string rootFolderPath = source.Path;

			if (directory != null)
			{
				destinationPath = Path.Combine(additionalFolders == null ? directory : Path.Combine(directory, additionalFolders),
					destinationPath);
				rootFolderPath = Path.Combine(rootFolderPath, directory);
			}

			string fullDestinationPath = Path.Combine(source.Path, destinationPath);

			Assert.IsFalse(File.Exists(destinationPath));

			scheduler.Builder.Append(new SourceRenamer(f => destinationPath));
			scheduler.Start();

			scheduler.ExecuteBlocking(source);

			Assert.IsTrue(File.Exists(fullDestinationPath));
			Assert.AreEqual(sourceSize, new FileInfo(fullDestinationPath).Length);

			File.Delete(fullDestinationPath);
			if (directory != null)
				Directory.Delete(rootFolderPath, true);
		}
	}
}