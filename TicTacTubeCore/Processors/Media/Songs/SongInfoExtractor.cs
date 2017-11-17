using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Utils;
using TicTacTubeCore.Utils.Extensions;

namespace TicTacTubeCore.Processors.Media.Songs
{
	/// <summary>
	/// A simple song info extractor that tries as hard as it can.
	/// </summary>
	public class SongInfoExtractor : IMediaInfoExtractor<SongInfo>
	{
		protected const string FeaturingRegex = @"\s?f(ea)?t\.?\s";

		protected string[] Delimiters = { @"\s-\s", @"\|" };
		protected string[] Preprocessors = { @"\[.*?\]", @"(?i)\([^)]*video\)" };

		protected string[] FeaturingStart = { FeaturingRegex };
		protected string[] ArtistSeperator = { @"\svs.?\s", @"\s&\s", @",\s", @"\swith\s" };
		protected string[] FeaturingEnd = { @"\)", FeaturingRegex };

		protected string[] PostProcessing = { @"\(\s*\)", FeaturingRegex };

		/// <summary>
		/// Determine whether the title should be used as album, if no album could be found.
		/// </summary>
		public bool UseTitleAsAlbum { get; set; } = true;

		/// <inheritdoc />
		public SongInfo Extract(IFileSource song)
		{
			string fileName = song.FileName;
			// todo: first try from metadata
			var songInfo = ExtractFromFileName(fileName);

			//TODO: implement
			throw new System.NotImplementedException();
			//return songInfo
		}

		protected virtual SongInfo ExtractFromFileName(string fileName)
		{
			var songInfo = new SongInfo();
			fileName = Preprocessors.Aggregate(fileName, (current, preprocessor) => Regex.Replace(current, preprocessor, ""));

			string[] split = null;

			// try all delemiters, and always split into two parts (before the first occurence, and after)
			foreach (string delimiter in Delimiters)
			{
				var parts = Regex.Split(fileName, delimiter);
				if (parts.Length > 1)
				{
					split = new string[2];
					split[0] = parts[0];

					string rest = "";
					for (int i = 1; i < parts.Length; i++)
					{
						rest += parts[i];
					}

					split[1] = rest;

					break;
				}
			}

			// split was successful
			if (split != null)
			{
				var a1 = SearchForArtists(split[0], true);
				var a2 = SearchForArtists(split[1], false);
			}

			return songInfo;
		}

		protected virtual IEnumerable<string> SearchForArtists(string input, bool artistsOnly)
		{
			var artists = new List<string>();

			var featuringParts = FindFeaturingParts(input, artistsOnly);

			foreach (string featuringPart in featuringParts)
			{
				artists.AddRange(RegexUtils.SplitMulti(featuringPart, ArtistSeperator));
			}

			return artists;
		}

		// search the string and find all new start indexes for featuring
		private IEnumerable<string> FindFeaturingParts(string input, bool artistsOnly)
		{
			// the indexes where a new autor line begins or ends
			var splitStartIndexes = new List<int>();
			var splitEndIndexes = new List<int>();

			if (artistsOnly)
			{
				splitStartIndexes.Add(0);
			}

			// test all start strings (starting a new artist) and store the indexes.
			StoreAllMatchIndexes(input, FeaturingStart, splitStartIndexes, true);

			// do the same for the end, although, we possibly do not need all ends
			StoreAllMatchIndexes(input, FeaturingEnd, splitEndIndexes, false);

			List<string> featuringParts = new List<string>();
			//TODO: beautify
			foreach (int splitStart in splitStartIndexes)
			{
				int? end = FindClosest(splitStart, splitEndIndexes);
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

				if (toBreak)
				{
					break;
				}
			}

			return featuringParts;
		}

		private void StoreAllMatchIndexes(string input, string[] regexes, List<int> indexes, bool addMatchOffset)
		{
			foreach (string regex in regexes)
			{
				StoreMatchIndexes(input, regex, indexes, addMatchOffset);
			}
			indexes.Sort();
		}

		private static void StoreMatchIndexes(string input, string regex, ICollection<int> splitStartIndexes, bool addMatchOffset)
		{
			var matches = Regex.Matches(input, regex);
			foreach (Match match in matches)
			{
				splitStartIndexes.Add(addMatchOffset ? match.Index + match.Length : match.Index);
			}
		}

		//todo: convert to extension method? if so, better name since it finds the first bigger
		private static int? FindClosest(int search, IEnumerable<int> toSearch)
		{
			// todo: if extension, sort toSearch
			foreach (int i in toSearch)
			{
				if (i > search)
				{
					return i;
				}
			}

			return null;
		}
	}
}