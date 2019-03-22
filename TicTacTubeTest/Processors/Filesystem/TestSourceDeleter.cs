using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Filesystem;
using TicTacTubeTest.Sources.Files;

namespace TicTacTubeTest.Processors.Filesystem
{
	[TestClass]
	public class TestSourceDeleter
	{
		[TestMethod]
		public void TestDeleteSource()
		{
			var scheduler = new SimpleTestScheduler();
			var source = new TempFileSource();

			scheduler.Builder.Append(new SourceDeleter());
			scheduler.Start();

			Assert.AreEqual(true, File.Exists(source.FileInfo.FullName));

			scheduler.ExecuteBlocking(source);

			Assert.AreEqual(false, File.Exists(source.FileInfo.FullName));
		}
	}
}