using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Processors.Media.Songs;

namespace TicTacTubeTest.Processors.Media.Songs
{
	[TestClass]
	public class TestSongInfoExtractor
	{
		[DataTestMethod]
		[DataRow("Selena Gomez, Marshmello - Wolves feat. Test & Test2 feat. Test3, Test4 & Test5", "Wolves", new[]
			{ "Selena Gomez", "Marshmello", "Test", "Test2", "Test3", "Test4", "Test5" })]
		[DataRow("WE ARE FURY - Waiting (feat. Olivia Lunny)", "Waiting", new[] { "WE ARE FURY", "Olivia Lunny" })]
		[DataRow("Laura Brehm - Breathe (Last Heroes & Crystal Skies Remix) (Lyric Video)",
			"Breathe (Last Heroes & Crystal Skies Remix)", new[] { "Laura Brehm" })]
		[DataRow("Rita Ora - Your Song(Official Lyric Video)", "Your Song", new[] { "Rita Ora" })]
		[DataRow("Rita Ora - Your Song(Official Video)", "Your Song", new[] { "Rita Ora" })]
		[DataRow("Dua Lipa - New Rules(Official Music Video)", "New Rules", new[] { "Dua Lipa" })]
		[DataRow("Snugs - Radio Silence (ft. HAILZ) [Lyric Video]", "Radio Silence", new[] { "Snugs", "HAILZ" })]
		[DataRow("Snugs - Radio Silence (feat HAILZ) [Lyric Video]", "Radio Silence", new[] { "Snugs", "HAILZ" })]
		[DataRow("Snugs & HAILZ - Radio Silence [Lyric Video]", "Radio Silence", new[] { "Snugs", "HAILZ" })]
		[DataRow("Snugs vs. HAILZ - Radio Silence [Lyric Video]", "Radio Silence", new[] { "Snugs", "HAILZ" })]
		[DataRow("Snugs ft.Edge1 ftEdge2 - Radio Silence [Lyric Video]", "Radio Silence", new[] { "Snugs", "Edge1 ftEdge2" })]
		[DataRow("Alan Walker - All Falls Down (feat. Noah Cyrus with Digital Farm Animals)", "All Falls Down",
			new[] { "Alan Walker", "Noah Cyrus", "Digital Farm Animals" })]
		[DataRow("Avicii - Lonely Together ft. Rita Ora", "Lonely Together", new[] { "Avicii", "Rita Ora" })]
		[DataRow("Slander - Superhuman [feat. Eric Leva] [Monstercat Release]", "Superhuman [Monstercat Release]",
			new[] { "Slander", "Eric Leva" })]
		[DataRow("Avicii - Without You “Audio” ft. Sandro Cavazza", "Without You", new[] { "Avicii", "Sandro Cavazza" })]
		[DataRow("David Guetta & Showtek - Bad ft.Vassy (Lyrics Video)", "Bad", new[] { "David Guetta", "Showtek", "Vassy" })]
		[DataRow("David Guetta - She Wolf (Falling To Pieces) ft. Sia (Official Video)", "She Wolf (Falling To Pieces)",
			new[] { "David Guetta", "Sia" })]
		[DataRow("David Guetta - Lovers On The Sun (Official Video) ft Sam Martin", "Lovers On The Sun",
			new[] { "David Guetta", "Sam Martin" })]
		[DataRow("David Guetta - Where Them Girls At ft. Nicki Minaj, Flo Rida (Official Video)", "Where Them Girls At",
			new[] { "David Guetta", "Nicki Minaj", "Flo Rida" })]
		[DataRow("Rihanna - Work (Explicit) ft. Drake", "Work", new[] { "Rihanna", "Drake" })]
		[DataRow("Basenji — Mistakes feat. Tkay Maidza", "Mistakes", new[] { "Basenji", "Tkay Maidza" })]
		[DataRow("Mistakes feat. Basenji & Tkay Maidza", "Mistakes", new[] { "Basenji", "Tkay Maidza" })]
		[DataRow("Mistakes feat. Basenji & Tkay Maidza [Test MashUp]", "Mistakes [Test MashUp]",
			new[] { "Basenji", "Tkay Maidza" })]
		[DataRow("Said The Sky ft. Missio - Nostalgia [Arrient Remix]", "Nostalgia [Arrient Remix]",
			new[] { "Said The Sky", "Missio" })]
		[DataRow("Said The Sky ft. Missio - Nostalgia [Test MashUp]", "Nostalgia [Test MashUp]",
			new[] { "Said The Sky", "Missio" })]
		[DataRow("Thor: Ragnarok Song | God Of Thunder | #NerdOut [Prod. by Boston]", "God Of Thunder #NerdOut",
			new[] { "Thor: Ragnarok Song" })]
		[DataRow("getting cozy ~ chill music mix ft.blackbear & jeremy zucker & eden", "chill music mix",
			new[] { "getting cozy", "blackbear", "jeremy zucker", "eden" })]
		[DataRow("Tritonal + Sj - Calabasas [Lyric Video]", "Calabasas", new[] { "Tritonal", "Sj" })]
		[DataRow("Don Diablo - People Say ft. Paije | Official Music Video", "People Say", new[] { "Don Diablo", "Paije" })]
		[DataRow("Marshmello & Anne-Marie - FRIENDS (Lyric Video) *OFFICIAL FRIENDZONE ANTHEM*", "FRIENDS",
			new[] { "Marshmello", "Anne-Marie" })]
		public void TestSongInfoExtractionByName(string input, string title, string[] artists)
		{
			var extracted = new SongInfoExtractor(false).ExtractFromStringAsyncTask(input).GetAwaiter().GetResult();
			Assert.AreEqual(title, extracted.Title);

			Assert.AreEqual(artists.Length, extracted.Artists.Length);

			for (int i = 0; i < artists.Length; i++)
			{
				Assert.AreEqual(artists[i], extracted.Artists[i]);
			}
		}
	}
}