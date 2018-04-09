using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Utils;

namespace TicTacTubeCoreTest.Utils
{
	[TestClass]
	public class TestArrayUtils
	{
		[DataTestMethod]
		[DataRow("hallo", 3)]
		[DataRow(3, 7)]
		[DataRow(4.20, 7)]
		public void TestMultiplex(object obj, int count)
		{
			var arr = ArrayUtils.Multiplex(obj, count);

			Assert.AreEqual(count, arr.Length);

			foreach (var elem in arr)
			{
				Assert.AreSame(obj, elem);
			}
		}
	}
}