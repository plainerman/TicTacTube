using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCoreTest.Sources.Files
{
	[TestClass]
	public class TestFileSource
	{
		private static IFileSource GenerateFileSource(string path) => new FileSource(path);

		[DataTestMethod]
		[DataRow(@"C:\", "test", ".txt")]
		[DataRow(@"\\192.168.0.1\public", "test.pdf", ".md")]
		public void TestFileInfo(string path, string filename, string extension)
		{
			var fileSource = GenerateFileSource(Path.Combine(path, filename + extension));

			Assert.AreEqual(filename, fileSource.FileName);
			Assert.AreEqual(extension, fileSource.FileExtension);
		}

		[TestMethod]
		public void TestBadFileInfo()
		{
			Assert.ThrowsException<ArgumentException>(() => GenerateFileSource(null));
			Assert.ThrowsException<ArgumentException>(() => GenerateFileSource(""));
			Assert.ThrowsException<ArgumentException>(() => GenerateFileSource("  "));
		}
	}
}