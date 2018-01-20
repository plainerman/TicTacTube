using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Media.Songs;

namespace TicTacTubeCoreTest.Processors.Media.Songs
{
	[TestClass]
	public class TestSongInfoMerger
	{
		[DataTestMethod]
		[DataRow("Love", "Lana Del Rey", "Love", "Beyoncé", false, "Love", "Lana Del Rey")]
		[DataRow("Your Song", "Rita Ora", "Your Song", "Elton John", false, "Your Song", "Rita Ora")]
		[DataRow("Dreamer", "Axwell _ Ingrosso", "Dreamer", "Axwell ^ Ingrosso", false, "Dreamer", "Axwell ^ Ingrosso")]
		[DataRow("A Different Way (Tritonal Remix)", "DJ Snake~Lauv", "A Different Way", "DJ Snake~Lauv", false,
			"A Different Way (Tritonal Remix)", "DJ Snake~Lauv")]
		[DataRow("IDGAF (CryJaxx & Marin Hoxha Remix)", "Dua Lipa", "IDGAF", "Dua Lipa", false,
			"IDGAF (CryJaxx & Marin Hoxha Remix)", "Dua Lipa")]
		[DataRow("Breathe", "Jax Jones~Ina Wroldsen", "Breathe", "Jax Jones~Ina Wroldsen", false, "Breathe",
			"Jax Jones~Ina Wroldsen")]
		[DataRow("Break Up Every Night (Dark Heart Remix)", "The Chainsmokers", "Break Up Every Night", "The Chainsmokers",
			false, "Break Up Every Night (Dark Heart Remix)", "The Chainsmokers")]
		[DataRow("Break Up Every Night (TYNAN Remix)", "The Chainsmokers", "Break Up Every Night", "The Chainsmokers", false,
			"Break Up Every Night (TYNAN Remix)", "The Chainsmokers")]
		public void TestSongInfoMerging(string title1, string artist1, string title2, string artist2, bool greedy,
			string newTitle, string newArtists)
		{
			const char splitChar = '~';
			var songInfo1 = new SongInfo { Title = title1, Artists = artist1.Split(splitChar) };
			var songInfo2 = new SongInfo { Title = title2, Artists = artist2.Split(splitChar) };

			var merged = new SongInfoMerger().Merge(songInfo1, songInfo2, greedy);
			Assert.AreEqual(newTitle, merged.Title);
			CollectionAssert.AreEqual(newArtists.Split(splitChar), merged.Artists);
		}
	}
}