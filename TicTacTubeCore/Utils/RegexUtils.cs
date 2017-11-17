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
			return SplitMulti(input, patterns, ArrayUtils.Multiplex(RegexOptions.None, patterns.Length));
		}

		public static string[] SplitMulti(string input, string[] patterns, RegexOptions[] options)
		{
			return SplitMulti(input, patterns, options, ArrayUtils.Multiplex(Timeout.InfiniteTimeSpan, patterns.Length));
		}

		public static string[] SplitMulti(string input, string[] patterns, RegexOptions[] options, TimeSpan[] matchTimeout)
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

			return Split(input, splits);
		}

		private static string[] Split(string input, IReadOnlyList<RegexSplit> splits)
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

		private struct RegexSplit : IComparable<RegexSplit>
		{
			public int StartIndex;
			public int MatchLength;

			public RegexSplit(int startIndex, int matchLength)
			{
				StartIndex = startIndex;
				MatchLength = matchLength;
			}

			public int CompareTo(RegexSplit other)
			{
				var startIndexComparison = StartIndex.CompareTo(other.StartIndex);
				if (startIndexComparison != 0) return startIndexComparison;
				return MatchLength.CompareTo(other.MatchLength);
			}
		}

	}

}