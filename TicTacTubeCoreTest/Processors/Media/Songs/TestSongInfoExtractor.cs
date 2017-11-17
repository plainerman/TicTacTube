using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Media.Songs;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCoreTest.Processors.Media.Songs
{
	[TestClass]
	public class TestSongInfoExtractor
	{
		[DataTestMethod]
		[DataRow("Selena Gomez, Marshmello - Wolves feat. Test & Test2 feat. Test3", "Wolves", new[] { "Selena Gomez", "Marshmello", "Test", "Test2", "Test3" })]
		[DataRow("WE ARE FURY - Waiting (feat. Olivia Lunny)", "Waiting", new[] { "WE ARE FURY", "Olivia Lunny" })]
		[DataRow("Laura Brehm - Breathe (Last Heroes & Crystal Skies Remix) (Lyric Video)", "Breathe", new[] { "Laura Brehm", "Last Heroes & Crystal Skies" })]
		[DataRow("Rita Ora - Your Song(Official Lyric Video)", "Your Song", new[] { "Rita Ora" })]
		[DataRow("Rita Ora - Your Song(Official Video)", "Your Song", new[] { "Rita Ora" })]
		[DataRow("Dua Lipa - New Rules(Official Music Video)", "New Rules", new[] { "Dua Lipa" })]
		[DataRow("Snugs - Radio Silence (ft. HAILZ) [Lyric Video]", "Radio Silence", new[] { "Snugs", "HAILZ" })]
		[DataRow("Snugs - Radio Silence (feat HAILZ) [Lyric Video]", "Radio Silence", new[] { "Snugs", "HAILZ" })]
		[DataRow("Snugs & HAILZ - Radio Silence [Lyric Video]", "Radio Silence", new[] { "Snugs", "HAILZ" })]
		[DataRow("Snugs vs. HAILZ - Radio Silence [Lyric Video]", "Radio Silence", new[] { "Snugs", "HAILZ" })]
		[DataRow("Alan Walker - All Falls Down (feat. Noah Cyrus with Digital Farm Animals)", "All Falls Down", new[] { "Alan Walker", "Noah Cyrus", "Digital Farm Animals" })]
		public void TestSongInfoExtraction(string input, string title, string[] artists)
		{
			//TODO: readd / think about other concept, once the file has to exist in order to be analyzed
			//because bitrate
			//var tmpFile = Path.GetTempFileName();
			//var testFile = Path.Combine(Path.GetDirectoryName(tmpFile), input);
			//File.Move(tmpFile, testFile);

			var extracted = new SongInfoExtractor().Extract(new FileSource(Path.Combine(Path.GetTempPath(), input + ".mp3")));
			Assert.AreEqual(title, extracted.Title);

			Assert.AreEqual(artists.Length, extracted.Artists.Length);

			for (var i = 0; i < artists.Length; i++)
			{
				Assert.AreEqual(artists[i], extracted.Artists[i]);
			}

			//File.Delete(testFile);
		}
	}
}