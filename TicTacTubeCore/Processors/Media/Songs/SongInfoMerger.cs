using System.Linq;
using System.Text.RegularExpressions;

namespace TicTacTubeCore.Processors.Media.Songs
{
	/// <summary>
	///     An <see cref="IMediaInfoMerger{T}" /> that merges two <see cref="SongInfo" />s based on their aritst and title
	///     similarity.
	/// </summary>
	public class SongInfoMerger : BaseMediaInfoMerger<SongInfo>
	{
		private const float TrustThreshold = 0.65f;

		/// <inheritdoc />
		public override SongInfo Merge(SongInfo info1, SongInfo info2, bool greedy)
		{
			CalculateTrustScore(info1, info2, out float artistTrust, out float titleTrust, out float combinedTrust);
			bool trusted = combinedTrust > TrustThreshold;

			// if its trusted, but neither the artists are trusted nor the title, then title and artists have to swapped (probably)
			// we have to assume, that the newer data fixes it
			if (trusted && info1.Artists.Length > 0 && artistTrust < TrustThreshold && titleTrust < TrustThreshold)
			{
				string oldTitle = info1.Title;
				info1.Title = info1.Artists[0];
				info1.Artists[0] = oldTitle;
				CalculateTrustScore(info1, info2, out artistTrust, out titleTrust, out combinedTrust);
			}

			// we require a higher title trust score
			if (titleTrust > TrustThreshold)
			{
				info1.Title = MergeData(info1.Title, info2.Title, trusted, greedy);
				info1.Album = MergeData(info1.Album, info2.Album, trusted, greedy);
			}

			info1.AlbumArtists = MergeDataArray(info1.AlbumArtists, info2.AlbumArtists, trusted, greedy);

			// if we trust the artists, they are probably the same and may contain spelling errors - so we fix them
			if (artistTrust > TrustThreshold)
				info1.Artists = MergeData(info1.Artists, info2.Artists, trusted, greedy);
			else
				info1.Artists = MergeDataArray(info1.Artists, info2.Artists, trusted, greedy);

			// since we normally only use a single image, we do not wanna compare.
			info1.Pictures = MergeData(info1.Pictures, info2.Pictures, trusted, greedy);
			info1.Year = MergeData(info1.Year, info2.Year, trusted, greedy);

			return info1;
		}

		/// <summary>
		///     Calculate the trust score (see <see cref="TestMatch" />) for three string pairs:
		///     <list type="bullet">
		///         <item>
		///             <description>The title of <paramref name="info1" /> and <paramref name="info2" />.</description>
		///         </item>
		///         <item>
		///             <description>The artists of <paramref name="info1" /> and <paramref name="info2" />.</description>
		///         </item>
		///         <item>
		///             <description>
		///                 The title and artists of <paramref name="info1" /> and <paramref name="info2" /> in order to
		///                 detect swapped elements.
		///             </description>
		///         </item>
		///     </list>
		///     The order of these infos does not matter.
		/// </summary>
		/// <param name="info1">The first info.</param>
		/// <param name="info2">The second info.</param>
		/// <param name="artistTrust">The match score for the artists.</param>
		/// <param name="titleTrust">The match score for the titles.</param>
		/// <param name="combinedTrust">The match score for a concatination of artists and title.</param>
		protected virtual void CalculateTrustScore(SongInfo info1, SongInfo info2, out float artistTrust,
			out float titleTrust, out float combinedTrust)
		{
			titleTrust = TestMatch(info1.Title, info2.Title);

			string artist1 = string.Join(" ", info1.Artists);
			string artist2 = string.Join(" ", info2.Artists);
			artistTrust = TestMatch(artist1, artist2);

			combinedTrust = TestMatch(info1.Title + " " + artist1, info2.Title + " " + artist2);
		}

		/// <summary>
		///     This method is used the calculate the match of two given strings.
		///     A value between [0;1] is returned — one is an absolute match and 0 no match.
		///     The average between the following two values is returned:
		///     <list type="number">
		///         <item>
		///             <description>
		///                 All space seperated items will be tokenized. Then they will be compared how many of these
		///                 completely match.
		///             </description>
		///         </item>
		///         <item>
		///             <description>
		///                 The length of the matched and unmatched tokens will be compared (i.e. the actual character
		///                 length not the token count as in 1.). So longer tokens will be punished more severly.
		///             </description>
		///         </item>
		///     </list>
		/// </summary>
		/// <param name="strA">The first string.</param>
		/// <param name="strB">The second string.</param>
		/// <param name="caseSensitve">Whether the comparison should be case sensitve or not.</param>
		/// <returns>A value between [0;1] is returned — one is an absolute match and 0 no match.</returns>
		protected virtual float TestMatch(string strA, string strB, bool caseSensitve = false)
		{
			if (string.IsNullOrWhiteSpace(strA) || string.IsNullOrWhiteSpace(strB)) return 0;

			if (!caseSensitve)
			{
				strA = strA.ToLower();
				strB = strB.ToLower();
			}

			strA = Regex.Replace(strA, @"\s+", " ").Trim();
			strB = Regex.Replace(strB, @"\s+", " ").Trim();

			var tokensA = strA.Split(' ').Distinct().ToArray();
			var tokensB = strB.Split(' ').Distinct().ToArray();

			// A\B n B\A
			var unmatchedTokens = tokensB.Except(tokensA).ToList();
			unmatchedTokens.AddRange(tokensA.Except(tokensB));

			var matchedTokens = tokensA.Intersect(tokensB).ToList();

			int matchedLength = matchedTokens.Sum(s => s.Length);
			int unmatchedLength = unmatchedTokens.Sum(s => s.Length);
			float tokenMatch = (float) matchedTokens.Count / (matchedTokens.Count + unmatchedTokens.Count);
			float lengthMatch = (float) matchedLength / (matchedLength + unmatchedLength);
			return (tokenMatch + lengthMatch) / 2;
		}
	}
}