using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Utils.Extensions.Strings;

namespace TicTacTubeCore.Processors.Media.Songs
{
	/// <summary>
	///     A simple song info extractor that tries as hard as it can to parse from the filename.
	/// </summary>
	public class SongInfoExtractor : IMediaInfoExtractor<SongInfo>
	{
		private const string FeaturingRegexWithoutSpace = @"f(ea)?t(\.?\s|\.)";

		/// <summary>
		///     The regex that matches featuring in song titles.
		/// </summary>
		protected const string FeaturingRegex = @"\s?" + FeaturingRegexWithoutSpace;

		/// <summary>
		///     The text that will be appended, if <see cref="UseTitleAsAlbum" /> is active.
		/// </summary>
		public const string Single = " (Single)";

		/// <summary>
		///     All delimiters that indicate another artist following.
		/// </summary>
		protected string[] ArtistSeperator = { @"(?i)\s(&|\+|x|with|vs.?)\s", @",\s" };

		/// <summary>
		///     Common delimiters for song titles (seperate songname from main artist)
		/// </summary>
		protected string[] Delimiters = { @"\s-\s", @"\s–\s", @"\s—\s", @"\|", @"~" };

		/// <summary>
		///     All delimiters that mark the end of a chain of artists.
		/// </summary>
		protected string[] FeaturingEnd = { @"(\(|\)|\[|\])", FeaturingRegex };

		/// <summary>
		///     All sequences that define a list of sequences. Also add those to the <see cref="PostProcessors" />.
		/// </summary>
		protected string[] FeaturingStart = { FeaturingRegex };

		/// <summary>
		///     The postprocessors that will be executed and deltete certain parts.
		/// </summary>
		protected string[] PostProcessors = { FeaturingRegexWithoutSpace, @"\(\s*\)" };

		/// <summary>
		///     The postprocessors that will be executed and deltete certain parts.
		/// </summary>
		protected IStringProcessor[] PostStringProcessors = { new SquareBracketsStringProcessor() };

		/// <summary>
		///     The preprocessors that will be executed and delete certain parts.
		/// </summary>
		protected string[] PreProcessors =
		{
			@"(?i)\s*\([^)]*(audio|video)\)", "(?i)(\"|“)audio(\"|”)", @"(?i)\s*\([^)]*explicit\)",
			@"(?i)\s*\([^)]*visualiser\)", @"(?i)\|\s*\(?[^)]*(audio|video)\)?"
		};

		/// <summary>
		///     Determine whether the title should be used as album, if no album could be found.
		/// </summary>
		public bool UseTitleAsAlbum { get; set; } = true;

		/// <summary>
		///     This method extracts songinfo from a given string (<paramref name="songTitle" />).
		///     Other features like bitrate won't be extracted here.
		///     It works with formatting like:
		///     Laura Brehm - Breathe (Last Heroes &amp; Crystal Skies Remix) (Lyric Video)
		/// </summary>
		/// <param name="songTitle">
		///     The string that should be as verbose as possible for the program to correctly identify the
		///     song.
		/// </param>
		/// <returns>A <see cref="SongInfo" /> containing the title and artists.</returns>
		public virtual async Task<SongInfo> ExtractFromStringAsyncTask(string songTitle) =>
			await Task.Run(() => ExtractFromString(songTitle));

		/// <summary>
		///     This method extracts songinfo from a given string (<paramref name="songTitle" />).
		///     Other features like bitrate won't be extracted here.
		///     It works with formatting like:
		///     Laura Brehm - Breathe (Last Heroes &amp; Crystal Skies Remix) (Lyric Video)
		/// </summary>
		/// <param name="songTitle">
		///     The string that should be as verbose as possible for the program to correctly identify the
		///     song.
		/// </param>
		/// <returns>A <see cref="SongInfo" /> containing the title and artists.</returns>
		public virtual SongInfo ExtractFromString(string songTitle)
		{
			var songInfo = new SongInfo();
			// apply the preprocessors
			songTitle = PreProcessors.Aggregate(songTitle, (current, preprocessor) => Regex.Replace(current, preprocessor, ""));

			string[] split = null;

			// try all delemiters, and always split into two parts (before the first occurence, and after)
			// Artist - Song name -> {"Artist", "Song name"}
			foreach (string delimiter in Delimiters)
			{
				var parts = Regex.Split(songTitle, delimiter);

				// if it could correctly split it
				if (parts.Length > 1)
				{
					split = new string[2];
					split[0] = parts[0];

					string remaining = "";
					for (int i = 1; i < parts.Length; i++)
					{
						remaining += parts[i];
					}

					split[1] = remaining;

					break;
				}
			}
			var artists = new List<string>();
			string titlePart;
			if (split != null)
			{
				artists.AddRange(SearchForArtists(ref split[0], true));
				titlePart = split[1];
			}
			else
			{
				titlePart = songTitle;
			}

			artists.AddRange(SearchForArtists(ref titlePart, false));

			songInfo.Artists = artists.ToArray();
			for (int i = 0; i < artists.Count; i++)
			{
				songInfo.Artists[i] = songInfo.Artists[i].Trim();
			}

			// apply the post processors
			songInfo.Title = PostProcessors
				.Aggregate(titlePart, (current, postprocessor) => Regex.Replace(current, postprocessor, ""));

			songInfo.Title = PostStringProcessors
				.Aggregate(songInfo.Title, (current, postprocessor) => postprocessor.Process(current));

			songInfo.Title = songInfo.Title.Trim();
			// remove multiple spaces
			songInfo.Title = Regex.Replace(songInfo.Title, @"\s+", " ");

			if (UseTitleAsAlbum)
				songInfo.Album = songInfo.Title + Single;

			return songInfo;
		}

		/// <summary>
		///     This method searches for artists in a given string and automatically removes the found artist from the given
		///     <paramref name="input" />.
		///     This method is intended to be used twice, once with the artist part of a song name, and once with the actual song
		///     name.
		///     In the song name it has to be searched for identifiers like feat. to start finding artists (set
		///     <paramref name="artistsOnly" /> to <c>false</c>).
		/// </summary>
		/// <param name="input">
		///     One of the two song identifier halfs. This string will not contain the artists after this
		///     operation.
		/// </param>
		/// <param name="artistsOnly">
		///     Determines, whether to look for an <see cref="FeaturingStart" /> to search for artists. If
		///     <c>true</c>, it will not search for <see cref="FeaturingStart" />.
		/// </param>
		/// <returns>A collection of all found artists.</returns>
		protected virtual IEnumerable<string> SearchForArtists(ref string input, bool artistsOnly)
		{
			var artists = new List<string>();

			var featuringParts = FindFeaturingParts(ref input, artistsOnly);

			foreach (string featuringPart in featuringParts)
			{
				artists.AddRange(RegexUtils.SplitMulti(featuringPart, ArtistSeperator));
			}

			return artists;
		}

		/// <summary>
		///     This method searches for multiple start indexes of featuring lists. So, essentially, all feat. ... .
		///     These are then returned and the input correctly adapted (to not contain the artists).
		/// </summary>
		/// <param name="input">
		///     One of the two song identifier halfs. This string will not contain the artists after this
		///     operation.
		/// </param>
		/// <param name="artistsOnly">
		///     Determines, whether to look for an <see cref="FeaturingStart" /> to search for artists. If
		///     <c>true</c>, it will not search for <see cref="FeaturingStart" />.
		/// </param>
		/// <returns>A collection of all found featuring parts (e.g. Marshmello, Porter Robinson).</returns>
		protected virtual IEnumerable<string> FindFeaturingParts(ref string input, bool artistsOnly)
		{
			// the indexes where a new autor line begins or ends
			var splitStartIndexes = new List<int>();
			var splitEndIndexes = new List<int>();

			if (artistsOnly)
				splitStartIndexes.Add(0);

			// test all start strings (starting a new artist) and store the indexes.
			StoreAllMatchIndexes(input, FeaturingStart, splitStartIndexes, true);

			// do the same for the end, although, we possibly do not need all ends
			StoreAllMatchIndexes(input, FeaturingEnd, splitEndIndexes, false);

			return SplitFeaturing(ref input, splitStartIndexes, splitEndIndexes);
		}

		/// <summary>
		///     This method gets the split indexes (indexes where featurings could start or could end), and will then create the
		///     actual featuring parts.
		///     This method is intended to be used by <see cref="FindFeaturingParts" /> that has already searched the parts.
		///     The featuring parts are stripped away from the input in this method.
		/// </summary>
		private IEnumerable<string> SplitFeaturing(ref string input, IEnumerable<int> splitStartIndexes,
			IReadOnlyCollection<int> splitEndIndexes)
		{
			var splits = new List<StringPosition>();

			var featuringParts = new List<string>();
			foreach (int splitStart in splitStartIndexes)
			{
				// find the closest matching end indexs (e.g. closes closing bracket)
				var end = FindClosest(splitStart, splitEndIndexes);
				int splitEnd;
				bool toBreak = false;
				if (end.HasValue)
				{
					splitEnd = end.Value;
				}
				else
				{
					splitEnd = input.Length;
					toBreak = true;
				}

				featuringParts.Add(input.SubstringByIndex(splitStart, splitEnd));

				splits.Add(new StringPosition(splitStart, splitEnd - splitStart));

				if (toBreak)
					break;
			}

			input = input.Remove(splits);

			return featuringParts;

			// Find the next biggest end index to a given split start.
			int? FindClosest(int search, IEnumerable<int> toSearch)
			{
				foreach (int i in toSearch)
				{
					if (i > search)
						return i;
				}

				return null;
			}
		}

		/// <summary>
		///     This method calls <see cref="StoreMatchIndexes" /> multiple times with a list of regexes and then sorts the
		///     indexes.
		///     This allows to find multiple split indexes that are sorted after their index.
		/// </summary>
		private static void StoreAllMatchIndexes(string input, IEnumerable<string> regexes, List<int> indexes,
			bool addMatchOffset)
		{
			foreach (string regex in regexes)
			{
				StoreMatchIndexes(input, regex, indexes, addMatchOffset);
			}

			indexes.Sort();
		}

		/// <summary>
		///     This method stores all matches of a regex into the given collection <paramref name="indexes" />.
		///     If <paramref name="addMatchOffset" /> is <c>true</c>, it skips the matched regex (e.g. feat. ab will start at index
		///     of ab).
		/// </summary>
		private static void StoreMatchIndexes(string input, string regex, ICollection<int> indexes, bool addMatchOffset)
		{
			var matches = Regex.Matches(input, regex);

			foreach (Match match in matches)
			{
				indexes.Add(addMatchOffset ? match.Index + match.Length : match.Index);
			}
		}

		/// <inheritdoc />
		public async Task<SongInfo> ExtractAsyncTask(IFileSource song)
		{
			string fileName = song.FileName;

			var songInfoFromFile = await SongInfo.ReadFromFileAsyncTask(song.FileInfo.FullName);
			var songInfo = await ExtractFromStringAsyncTask(fileName);

			songInfoFromFile.Title = songInfo.Title;
			songInfoFromFile.Artists = songInfo.Artists;

			if (string.IsNullOrEmpty(songInfoFromFile.Album))
				songInfoFromFile.Album = songInfo.Album;

			return songInfoFromFile;
		}
	}
}