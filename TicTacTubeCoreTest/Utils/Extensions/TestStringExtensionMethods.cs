using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Utils.Extensions;

namespace TicTacTubeCoreTest.Utils.Extensions
{
	[TestClass]
	public class TestStringExtensionMethods
	{
		[DataTestMethod]
		[DataRow(@"Game over", @"e", @"3", 0, @"Gam3 over")]
		[DataRow(@"Game over", @"e", @"3", 3, @"Gam3 over")]
		[DataRow(@"Game over", @"e", @"3", 4, @"Game ov3r")]
		[DataRow(@"ooo", @"o", @"0", 0, @"0oo")]
		[DataRow(@"ooo", @"o", @"0", 1, @"o0o")]
		[DataRow(@"ooo", @"o", @"0", 2, @"oo0")]
		[DataRow(@"ooo", @"o", @"0", 3, @"ooo")]
		public void TestReplaceFirst(string baseString, string search, string replace, int startIndex, string result)
		{
			Assert.AreEqual(result, baseString.ReplaceFirst(search, replace, startIndex));
		}

		[DataTestMethod]
		[DataRow(@"Game over", 0, 1, @"G")]
		[DataRow(@"Game over", 0, 2, @"Ga")]
		[DataRow(@"Game over", 1, 3, @"am")]
		public void TestSubstringByIndex(string baseString, int start, int end, string result)
		{
			Assert.AreEqual(result, baseString.SubstringByIndex(start, end));
		}
	}
}