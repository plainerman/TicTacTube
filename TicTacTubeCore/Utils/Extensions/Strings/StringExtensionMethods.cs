using System;
using System.Collections.Generic;

namespace TicTacTubeCore.Utils.Extensions.Strings
{
	/// <summary>
	/// This class is for useful string extensions.
	/// </summary>
	public static class StringExtensionMethods
	{
		/// <summary>
		/// This method replaces the first occurence of a given string, beginning from a given start position.
		/// </summary>
		/// <param name="text">The string this operation is based on.</param>
		/// <param name="search">The search string.</param>
		/// <param name="replace">The string that will be replaced.</param>
		/// <param name="startIndex">The index from where to begin.</param>
		/// <returns>A new string with the replaced value.</returns>
		public static string ReplaceFirst(this string text, string search, string replace, int startIndex = 0)
		{
			int pos = text.IndexOf(search, startIndex, StringComparison.Ordinal);
			if (pos < 0) return text;

			return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
		}
		/// <summary>
		/// This method returns a substring with a given start (inclusive) and end position (exclusive).
		/// </summary>
		/// <param name="text">The string this operation is based on.</param>
		/// <param name="start">The start index.</param>
		/// <param name="end">The end index.</param>
		/// <returns>A new substring inclusive start- exclusive endpos.</returns>
		public static string SubstringByIndex(this string text, int start, int end)
		{
			return text.Substring(start, Math.Min(text.Length, end) - start);
		}

		//TODO: test
		/// <summary>
		/// This method splits a given string on multiple string positions, where the individual string positions are removed.
		/// </summary>
		/// <param name="text">The string this operation is based on.</param>
		/// <param name="splits">Multiple splits that define the position of the splits.</param>
		/// <returns>A new string with the split values.</returns>
		public static string[] Split(this string text, IReadOnlyList<StringPosition> splits)
		{
			var ret = new string[splits.Count + 1];
			for (int i = 0; i < ret.Length; i++)
			{
				int start = 0;
				int end = text.Length;

				if (i > 0)
				{
					var prevSplit = splits[i - 1];
					start = prevSplit.StartIndex + prevSplit.MatchLength;
				}
				if (i < splits.Count)
				{
					end = splits[i].StartIndex;
				}

				ret[i] = text.SubstringByIndex(start, end);
			}
			return ret;
		}

		//TODO: test
		/// <summary>
		/// This method allows to remove multiple sets of strings, and automatically updates the following split values to the new ones. (new indexes after some parts are removed)
		/// If the split ranges overlap, <b>undefined behavior</b> will occur.
		/// </summary>
		/// <param name="text">The string this operation is based on.</param>
		/// <param name="splits">Multiple splits that define the positions that will be removed.</param>
		/// <returns></returns>
		public static string Remove(this string text, IReadOnlyList<StringPosition> splits)
		{
			var localSplit = new List<StringPosition>(splits);
			localSplit.Sort();

			for (int i = 0; i < localSplit.Count; i++)
			{
				text = text.Remove(localSplit[i].StartIndex, localSplit[i].MatchLength);

				int difference = localSplit[i].MatchLength;
				for (int j = i + 1; j < localSplit.Count; j++)
				{
					localSplit[j] = new StringPosition(localSplit[j].StartIndex - difference, localSplit[j].MatchLength);
				}
			}

			return text;
		}
	}
}