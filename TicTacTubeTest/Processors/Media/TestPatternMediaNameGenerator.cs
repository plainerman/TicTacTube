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
		private const uint Year = 1998;
		private const string YearAsString = "1998";

		private const string ArtistA = "Michael Plainer";
		private const string ArtistB = "Marshmello";
		private static readonly string[] Artists = { ArtistA, ArtistB };

		private readonly SongInfo _info = new SongInfo
		{
			Album = Album,
			Title = Title,
			Year = Year,
			Artists = Artists
		};

		[DataTestMethod]
		[DataRow("{Album} - {Title}", Album + " - " + Title)]
		[DataRow("{Album} - {Title} - {AlbumArtists}", Album + " - " + Title + " - ")]
		[DataRow("{Album} - {Title} {Title}", Album + " - " + Title + " " + Title)]
		[DataRow("{Album}({Year}) - {Title}", Album + "(" + YearAsString + ") - " + Title)]
		[DataRow("{Album}({Year}) - {Title} - {Artists}",
			Album + "(" + YearAsString + ") - " + Title + " - " + ArtistA + ", " + ArtistB)]
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