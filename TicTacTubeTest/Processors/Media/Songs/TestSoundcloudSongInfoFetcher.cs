using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Media.Songs;
using TicTacTubeCore.Soundcloud.Processors.Media.Songs;
using TicTacTubeCore.Soundcloud.Processors.Media.Songs.Exceptions;

// This disables the warnings for missing awaits
#pragma warning disable 4014

namespace TicTacTubeTest.Processors.Media.Songs
{
	[TestClass]
	public class TestSoundcloudSongInfoFetcher
	{
		[DataTestMethod]
		[DataRow("https://soundcloud.com/disciple/normalitea", "Normalitea", "Dodge", "Dubstep")]
		[DataRow("https://soundcloud.com/foxstevenson/fox-stevenson-melange-1", "Melange", "Fox Stevenson", "Drum")]
		[DataRow("https://soundcloud.com/monstercat/pegboard-nerds-party-freaks", "Party Freaks", "Pegboard Nerds",
			"Electro")]
		[DataRow("https://soundcloud.com/monstercat/puppet-first-time-fighting", "First Time Fighting", "Puppet",
			"Electronic")]
		public void TestExtractFromUrl(string url, string title, string firstArtist, string firstGenre)
		{
			var fetcher = new SoundcloudSongInfoFetcher();
			var result = new SongInfo();

			try
			{
				result = fetcher.ExtractFromStringAsyncTask(url).GetAwaiter().GetResult();
			}
			catch (WebException e)
			{
				Assert.Inconclusive("An error with the connection occured. The state of this test is inconclusive.", e);
			}

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
			try
			{
				fetcher.ExtractFromStringAsyncTask(badUrl).GetAwaiter().GetResult();
			}
			catch (InvalidSoundcloudPageTypeException) // expected
			{
				return;
			}
			catch (Exception e)
			{
				Assert.Inconclusive("This test requires an internet connection. Manually check the error.", e);
			}

			Assert.Fail("No exception has been thrown.");
		}
	}
}