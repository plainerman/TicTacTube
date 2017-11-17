using System;

namespace TicTacTubeCore.Utils.Extensions
{
	/// <summary>
	/// This class is for useful string extensions.
	/// </summary>
	public static class StringExtensionMethods
	{
		/// <summary>
		/// This method replaces the first occurence of a given string, beginning from a given start position.
		/// </summary>
		/// <param name="text">The string to test.</param>
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
		/// <param name="text">The string to test.</param>
		/// <param name="start">The start index.</param>
		/// <param name="end">The end index.</param>
		/// <returns>A new substring inclusive start- exclusive endpos.</returns>
		public static string SubstringByIndex(this string text, int start, int end)
		{

			return text.Substring(start, Math.Min(text.Length, end) - start);
		}
	}
}