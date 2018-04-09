using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Filesystem;
using TicTacTubeCoreTest.Sources.Files;

namespace TicTacTubeCoreTest.Processors.Filesystem
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

			Assert.AreEqual(true, File.Exists(source.FileInfo.FullName));

			scheduler.Execute(source);

			Assert.AreEqual(false, File.Exists(source.FileInfo.FullName));
		}
	}
}