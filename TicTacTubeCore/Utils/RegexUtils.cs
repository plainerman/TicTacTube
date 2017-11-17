using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using TicTacTubeCore.Utils.Extensions;

namespace TicTacTubeCore.Utils
{
	/// <summary>
	/// A convenient utils class that provides useful functions for regex usage.
	/// </summary>
	public static class RegexUtils
	{
		public static string[] SplitMulti(string input, string[] patterns)
		{
			return SplitMulti(input, patterns, ArrayUtils.Multiplex(RegexOptions.None, patterns.Length), out _);
		}

		public static string[] SplitMulti(string input, string[] patterns, out IList<RegexSplit> foundSplits)
		{
			return SplitMulti(input, patterns, ArrayUtils.Multiplex(RegexOptions.None, patterns.Length), out foundSplits);
		}

		public static string[] SplitMulti(string input, string[] patterns, RegexOptions[] options, out IList<RegexSplit> foundSplits)
		{
			return SplitMulti(input, patterns, options, ArrayUtils.Multiplex(Timeout.InfiniteTimeSpan, patterns.Length), out foundSplits);
		}

		public static string[] SplitMulti(string input, string[] patterns, RegexOptions[] options, TimeSpan[] matchTimeout, out IList<RegexSplit> foundSplits)
		{
			if (patterns.Length != options.Length)
				throw new ArgumentException("Not the correct amount of options supplied.", nameof(options));
			if (patterns.Length != matchTimeout.Length)
				throw new ArgumentException("Not the correct amount of matchTimeouts supplied.", nameof(matchTimeout));

			var splits = new List<RegexSplit>();

			for (int i = 0; i < patterns.Length; i++)
			{
				var matches = Regex.Matches(input, patterns[i], options[i], matchTimeout[i]);
				foreach (Match match in matches)
				{
					splits.Add(new RegexSplit(match.Index, match.Length));
				}
			}

			splits.Sort();
			foundSplits = splits;

			return Split(input, splits);
		}

		// TODO: test
		// TODO: to string utils?!?
		public static string StringRemove(string input, IReadOnlyList<RegexSplit> splits)
		{
			var localSplit = new List<RegexSplit>(splits);
			localSplit.Sort();

			//TODO: postulate to not overlap? probably the easier option

			for (int i = 0; i < localSplit.Count; i++)
			{
				input = input.Remove(localSplit[i].StartIndex, localSplit[i].MatchLength);

				int difference = localSplit[i].MatchLength;
				for (int j = i + 1; j < localSplit.Count; j++)
				{
					//TODO: check off by one
					localSplit[j] = new RegexSplit(localSplit[j].StartIndex - difference, localSplit[j].MatchLength);
				}
				//TODO: adjust other splits
			}

			return input;
		}


		public static string[] Split(string input, IReadOnlyList<RegexSplit> splits)
		{
			string[] ret = new string[splits.Count + 1];
			for (int i = 0; i < ret.Length; i++)
			{
				int start = 0;
				int end = input.Length;

				if (i > 0)
				{
					var prevSplit = splits[i - 1];
					start = prevSplit.StartIndex + prevSplit.MatchLength;
				}
				if (i < splits.Count)
				{
					end = splits[i].StartIndex;
				}

				ret[i] = input.SubstringByIndex(start, end);
			}
			return ret;
		}

		//TODO: better name ? Like string pos
		//TODO: to string utils?

		/// <summary>
		/// A simple struct that contains a regex split information.
		/// </summary>
		public struct RegexSplit : IComparable<RegexSplit>
		{
			/// <summary>
			/// The start index where the match occured.
			/// </summary>
			public int StartIndex;
			/// <summary>
			/// The length of the matched string.
			/// </summary>
			public int MatchLength;

			/// <summary>
			/// Create a new split information with a given <paramref name="startIndex"/> and <paramref name="matchLength"/>.
			/// </summary>
			/// <param name="startIndex">The start index where the match occured.</param>
			/// <param name="matchLength">The length of the matched string.</param>
			public RegexSplit(int startIndex, int matchLength)
			{
				StartIndex = startIndex;
				MatchLength = matchLength;
			}

			/// <inheritdoc />
			public int CompareTo(RegexSplit other)
			{
				int startIndexComparison = StartIndex.CompareTo(other.StartIndex);
				return startIndexComparison != 0 ? startIndexComparison : MatchLength.CompareTo(other.MatchLength);
			}
		}

	}

}