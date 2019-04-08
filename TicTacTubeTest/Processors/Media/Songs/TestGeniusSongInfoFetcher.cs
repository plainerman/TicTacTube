using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TicTacTubeCore.Genius.Processors.Media.Songs;
using TicTacTubeCore.Processors.Media.Songs;

namespace TicTacTubeTest.Processors.Media.Songs
{
	[TestClass]
	public class TestGeniusSongInfoFetcher
	{
		/// <summary>
		///     The path to the token file containing the Genius API-Key.
		/// </summary>
		public const string TokenFile = "genius.token";

		protected static GeniusSongInfoFetcher GeniusSongInfoFetcher;

		[ClassInitialize]
		public static void CreateFetcher(TestContext context)
		{
			string secret = null;

			if (File.Exists(TokenFile))
			{
				secret = File.ReadAllText(TokenFile);
			}

			GeniusSongInfoFetcher = new GeniusSongInfoFetcher(secret);
		}

		[DataTestMethod]
		[DataRow("Havana", "Camila Cabello", true)]
		[DataRow("Alone", "Marshmello", true)]
		[DataRow("Intake", "Stevie McFly", false)]
		public void TestExtractFromUrl(string title, string artist, bool hasCoverArt)
		{
			var inputInfo = new SongInfo
			{
				Title = title,
				Artists = new[] { artist }
			};

			var returnedInfo = new SongInfo();

			try
			{
				returnedInfo = GeniusSongInfoFetcher.ExtractAsyncTask(inputInfo).GetAwaiter().GetResult();
			}
			catch (Exception e)
			{
				Assert.Inconclusive("This test requires an internet connection. Manually check the error.", e);
			}

			Assert.AreEqual(inputInfo.Title.ToLower(), returnedInfo.Title.ToLower());

			Assert.AreEqual(inputInfo.Artists[0].ToLower(), returnedInfo.Artists[0].ToLower());

			if (hasCoverArt)
				Assert.AreNotEqual(0, returnedInfo.Pictures.Length);
			else
				Assert.AreEqual(0, returnedInfo.Pictures.Length);
		}
	}
}