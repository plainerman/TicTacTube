using System;

namespace TicTacTubeCore.Utils.Extensions.Strings
{
	/// <summary>
	///     A simple struct that contains a regex split information.
	/// </summary>
	public struct StringPosition : IComparable<StringPosition>
	{
		/// <summary>
		///     The start index where the match occured.
		/// </summary>
		public int StartIndex;

		/// <summary>
		///     The length of the matched string.
		/// </summary>
		public int MatchLength;

		/// <summary>
		///     Create a new split information with a given <paramref name="startIndex" /> and <paramref name="matchLength" />.
		/// </summary>
		/// <param name="startIndex">The start index where the match occured.</param>
		/// <param name="matchLength">The length of the matched string.</param>
		public StringPosition(int startIndex, int matchLength)
		{
			StartIndex = startIndex;
			MatchLength = matchLength;
		}

		/// <inheritdoc />
		public int CompareTo(StringPosition other)
		{
			int startIndexComparison = StartIndex.CompareTo(other.StartIndex);
			return startIndexComparison != 0 ? startIndexComparison : MatchLength.CompareTo(other.MatchLength);
		}
	}
}