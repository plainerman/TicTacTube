using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace TicTacTubeCore.Utils.Extensions.Strings
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

		public static string[] SplitMulti(string input, string[] patterns, out IList<StringPosition> foundSplits)
		{
			return SplitMulti(input, patterns, ArrayUtils.Multiplex(RegexOptions.None, patterns.Length), out foundSplits);
		}

		public static string[] SplitMulti(string input, string[] patterns, RegexOptions[] options, out IList<StringPosition> foundSplits)
		{
			return SplitMulti(input, patterns, options, ArrayUtils.Multiplex(Timeout.InfiniteTimeSpan, patterns.Length), out foundSplits);
		}

		public static string[] SplitMulti(string input, string[] patterns, RegexOptions[] options, TimeSpan[] matchTimeout, out IList<StringPosition> foundSplits)
		{
			if (patterns.Length != options.Length)
				throw new ArgumentException("Not the correct amount of options supplied.", nameof(options));
			if (patterns.Length != matchTimeout.Length)
				throw new ArgumentException("Not the correct amount of matchTimeouts supplied.", nameof(matchTimeout));

			var splits = new List<StringPosition>();

			for (int i = 0; i < patterns.Length; i++)
			{
				var matches = Regex.Matches(input, patterns[i], options[i], matchTimeout[i]);
				foreach (Match match in matches)
				{
					splits.Add(new StringPosition(match.Index, match.Length));
				}
			}

			splits.Sort();
			foundSplits = splits;

			return input.Split(splits);
		}
	}

}