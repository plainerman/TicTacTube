using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Soundcloud.Processors.Media.Songs;
using TicTacTubeCore.Soundcloud.Processors.Media.Songs.Exceptions;
// This disables the warnings for missing awaits
#pragma warning disable 4014

namespace TicTacTubeCoreTest.Processors.Media.Songs
{
	[TestClass]
	public class TestSoundcloudSongInfoFetcher
	{
		[DataTestMethod]
		[DataRow("https://soundcloud.com/disciple/normalitea", "Normalitea", "Dodge", "Dubstep")]
		[DataRow("https://soundcloud.com/foxstevenson/fox-stevenson-melange-1", "Melange", "Fox Stevenson", "Drum")]
		[DataRow("https://soundcloud.com/monstercat/pegboard-nerds-party-freaks", "Party Freaks", "Pegboard Nerds", "Electro")]
		[DataRow("https://soundcloud.com/monstercat/puppet-first-time-fighting", "First Time Fighting", "Puppet", "Electronic")]
		public void TestExtractFromUrl(string url, string title, string firstArtist, string firstGenre)
		{
			var fetcher = new SoundcloudSongInfoFetcher();
			var result = fetcher.ExtractFromStringAsyncTask(url).GetAwaiter().GetResult();

			Assert.AreEqual(title, result.Title);
			Assert.AreEqual(firstArtist, result.Artists[0]);
			Assert.AreEqual(firstGenre, result.Genres[0]);
			Assert.IsNotNull(result.Pictures);
			Assert.AreNotEqual(0, result.Pictures.Length);
		}

		[DataTestMethod]
		[DataRow("https://google.com")]
		[DataRow("https://soundcloud.com/")]
		[DataRow("https://soundcloud.com/foxstevenson/")]
		public void TestExtractFromBadUrl(string badUrl)
		{
			var fetcher = new SoundcloudSongInfoFetcher();
			Assert.ThrowsExceptionAsync<InvalidSoundcloudPageTypeException>(() => fetcher.ExtractFromStringAsyncTask(badUrl)).GetAwaiter().GetResult();
		}
	}
}