using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TicTacCore.Sources.Files;

namespace TicTacCoreTest.Sources.Files
{
	[TestClass]
	public class TestFileSource
	{
		private static IFileSource GenerateFileSource(string path)
		{
			return new FileSource(path);
		}

		[DataTestMethod]
		[DataRow(@"C:\", "test", ".txt")]
		[DataRow(@"\\192.168.0.1\public", "test.pdf", ".md")]
		public void TestFileInfo(string path, string filename, string extension)
		{
			IFileSource fileSource = GenerateFileSource(Path.Combine(path, filename + extension));

			Assert.AreEqual(filename, fileSource.FileName);
			Assert.AreEqual(extension, fileSource.FileExtension);
			Assert.AreEqual(path, fileSource.Path);
		}
	}
}