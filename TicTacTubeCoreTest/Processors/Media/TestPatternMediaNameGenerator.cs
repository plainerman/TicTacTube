using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Media;
using TicTacTubeCore.Processors.Media.Songs;

namespace TicTacTubeCoreTest.Processors.Media
{
	[TestClass]
	public class TestPatternMediaNameGenerator
	{
		private const string Album = "TestAlbum";
		private const string Title = "My awesome jam";
		private const string Year = "1998";

		private readonly SongInfo _info = new SongInfo
		{
			Album = Album,
			Title = Title,
			Year = Year
		};

		[DataTestMethod]
		[DataRow("{Album} - {Title}", Album + " - " + Title)]
		[DataRow("{Album} - {Title} {Title}", Album + " - " + Title + " " + Title)]
		[DataRow("{Album}({Year}) - {Title}", Album + "(" + Year + ") - " + Title)]
		public void TestPatternParse(string pattern, string expected)
		{
			Assert.AreEqual(expected, new PatternMediaNameGenerator<SongInfo>(pattern).Parse(_info));
		}

		[DataTestMethod]
		[DataRow("{Album - {Title}")]
		[DataRow("{Album}(Year}) - {Title}")]
		[DataRow("{Album}({Yearr}) - {Title}")]
		public void TestBadPattern(string pattern)
		{
			Assert.ThrowsException<FormatException>(() => new PatternMediaNameGenerator<SongInfo>(pattern));
		}
	}
}