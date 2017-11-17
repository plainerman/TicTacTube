using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Utils.Extensions.Strings;

namespace TicTacTubeCoreTest.Utils
{
	[TestClass]
	public class TestRegexUtils
	{
		private const string BasicTest = "This is-a,simple-test";

		[DataTestMethod]
		[DataRow(BasicTest, new[] { @"\s" }, new[] { "This", "is-a,simple-test" })]
		[DataRow(BasicTest, new[] { @"\s", "-" }, new[] { "This", "is", "a,simple", "test" })]
		[DataRow(BasicTest, new[] { @"\s", "," }, new[] { "This", "is-a", "simple-test" })]
		[DataRow(BasicTest, new[] { @"\s", "-", "," }, new[] { "This", "is", "a", "simple", "test" })]
		public void TestSplitMulti(string input, string[] patterns, string[] result)
		{
			var actualResult = RegexUtils.SplitMulti(input, patterns);

			Assert.AreEqual(result.Length, actualResult.Length);

			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(result[i], actualResult[i]);
			}
		}
	}
}