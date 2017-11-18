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
		/// <summary>
		/// This method works similar to <see cref="Regex.Split(string)"/> only that it is capable of splitting on multiple <paramref name="patterns"/>.
		/// None of the patterns have <see cref="RegexOptions"/> specified. The are no timeouts. 
		/// </summary>
		/// <param name="input">The input that will be split on given patterns.</param>
		/// <param name="patterns">The patterns that are used to split the input.</param>
		/// <returns>An array of the split <paramref name="input"/>.</returns>
		public static string[] SplitMulti(string input, string[] patterns)
		{
			return SplitMulti(input, patterns, ArrayUtils.Multiplex(RegexOptions.None, patterns.Length), out _);
		}

		/// <summary>
		/// This method works similar to <see cref="Regex.Split(string)"/> only that it is capable of splitting on multiple <paramref name="patterns"/>.
		/// None of the patterns have <see cref="RegexOptions"/> specified. The are no timeouts. 
		/// </summary>
		/// <param name="input">The input that will be split on given patterns.</param>
		/// <param name="patterns">The patterns that are used to split the input.</param>
		/// <param name="foundSplits">The splits that have been found and can be used to identify the split positions.</param>
		/// <returns>An array of the split <paramref name="input"/>.</returns>
		public static string[] SplitMulti(string input, string[] patterns, out IList<StringPosition> foundSplits)
		{
			return SplitMulti(input, patterns, ArrayUtils.Multiplex(RegexOptions.None, patterns.Length), out foundSplits);
		}

		/// <summary>
		/// This method works similar to <see cref="Regex.Split(string)"/> only that it is capable of splitting on multiple <paramref name="patterns"/>.
		/// Each pattern has it's own <paramref name="options"/>. The are no timeouts. 
		/// </summary>
		/// <param name="input">The input that will be split on given patterns.</param>
		/// <param name="patterns">The patterns that are used to split the input.</param>
		/// <param name="options">The options for the individual patterns.</param>
		/// <param name="foundSplits">The splits that have been found and can be used to identify the split positions.</param>
		/// <returns>An array of the split <paramref name="input"/>.</returns>
		public static string[] SplitMulti(string input, string[] patterns, RegexOptions[] options, out IList<StringPosition> foundSplits)
		{
			return SplitMulti(input, patterns, options, ArrayUtils.Multiplex(Timeout.InfiniteTimeSpan, patterns.Length), out foundSplits);
		}

		/// <summary>
		/// This method works similar to <see cref="Regex.Split(string)"/> only that it is capable of splitting on multiple <paramref name="patterns"/>.
		/// Each pattern has it's own <paramref name="options"/> and <paramref name="matchTimeouts"/>. 
		/// </summary>
		/// <param name="input">The input that will be split on given patterns.</param>
		/// <param name="patterns">The patterns that are used to split the input.</param>
		/// <param name="options">The options for the individual patterns.</param>
		/// <param name="matchTimeouts">The timeouts for the individual patterns.</param>
		/// <param name="foundSplits">The splits that have been found and can be used to identify the split positions.</param>
		/// <returns>An array of the split <paramref name="input"/>.</returns>
		public static string[] SplitMulti(string input, string[] patterns, RegexOptions[] options, TimeSpan[] matchTimeouts, out IList<StringPosition> foundSplits)
		{
			if (patterns.Length != options.Length)
				throw new ArgumentException("Not the correct amount of options supplied.", nameof(options));
			if (patterns.Length != matchTimeouts.Length)
				throw new ArgumentException("Not the correct amount of matchTimeouts supplied.", nameof(matchTimeouts));

			var splits = new List<StringPosition>();

			for (int i = 0; i < patterns.Length; i++)
			{
				var matches = Regex.Matches(input, patterns[i], options[i], matchTimeouts[i]);
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