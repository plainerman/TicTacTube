using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Utils.Extensions.Strings;

namespace TicTacTubeTest.Utils.Extensions.Strings
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

		[DataTestMethod]
		[DataRow("GamSPAMe over", new[] { 3, 4 }, "Game over")]
		[DataRow("GamSPAMe ovSPAMer", new[] { 3, 4, 11, 4 }, "Game over")]
		[DataRow("GamSPAMe ovSPAMerSPAMSPAM", new[] { 3, 4, 11, 4, 17, 4, 21, 4 }, "Game over")]
		public void TestRemove(string baseString, int[] positions, string result)
		{
			Assert.AreEqual(result, baseString.Remove(ToStringPos(positions)));
		}

		[DataTestMethod]
		[DataRow("Game over", new[] { 4, 1 }, new[] { "Game", "over" })]
		[DataRow("Game over", new[] { 1, 0, 4, 1 }, new[] { "G", "ame", "over" })]
		[DataRow("Game over", new[] { 1, 1, 4, 1, 6, 2 }, new[] { "G", "me", "o", "r" })]
		public void TestSplit(string baseString, int[] positions, string[] result)
		{
			var actualResult = baseString.Split(ToStringPos(positions.ToList()).ToList());

			Assert.AreEqual(result.Length, actualResult.Length);

			for (int i = 0; i < result.Length; i++)
			{
				Assert.AreEqual(result[i], actualResult[i]);
			}
		}

		private static IEnumerable<StringPosition> ToStringPos(IReadOnlyList<int> positions)
		{
			var stringPos = new StringPosition[positions.Count / 2];
			for (int i = 0; i < stringPos.Length; i++)
			{
				stringPos[i] = new StringPosition(positions[2 * i], positions[2 * i + 1]);
			}
			return stringPos;
		}
	}
}