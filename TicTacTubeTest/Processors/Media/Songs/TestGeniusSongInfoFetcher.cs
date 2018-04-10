using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Genius.Processors.Media.Songs;
using TicTacTubeCore.Processors.Media.Songs;

namespace TicTacTubeTest.Processors.Media.Songs
{
	[TestClass]
	public class TestGeniusSongInfoFetcher
	{
		/// <summary>
		/// The path to the token file containing the Genius API-Key.
		/// </summary>
		public const string TokenFile = "genius.token";
		/// <summary>
		/// The name of the environmental variable containing the Genius API-Key
		/// </summary>
		public const string SecretVariableName = "GeniusApiKey";

		protected static GeniusSongInfoFetcher GeniusSongInfoFetcher;

		[ClassInitialize]
		public static void CreateFetcher(TestContext context)
		{
			string secret = File.Exists(TokenFile) ? File.ReadAllText(TokenFile) : System.Environment.GetEnvironmentVariable(SecretVariableName);

			if (secret?.Trim().Length > 0)
			{
				GeniusSongInfoFetcher = new GeniusSongInfoFetcher(secret);
			}
			else
			{
				Assert.Fail("Genius token could not be loaded.");
			}
		}

		[DataTestMethod]
		[DataRow("Havana", "Camila Cabello", true)]
		[DataRow("Friends", "Marshmello", true)]
		[DataRow("Intake", "Stevie McFly", false)]
		public void TestExtractFromUrl(string title, string artist, bool hasCoverArt)
		{
			var inputInfo = new SongInfo
			{
				Title = title,
				Artists = new[] { artist }
			};

			var returnedInfo = GeniusSongInfoFetcher.ExtractAsyncTask(inputInfo).GetAwaiter().GetResult();

			Assert.AreEqual(inputInfo.Title.ToLower(), returnedInfo.Title.ToLower());

			Assert.AreEqual(inputInfo.Artists[0].ToLower(), returnedInfo.Artists[0].ToLower());

			if (hasCoverArt)
			{
				Assert.AreNotEqual(0, returnedInfo.Pictures.Length);
			}
			else
			{
				Assert.AreEqual(0, returnedInfo.Pictures.Length);
			}
		}
	}
}