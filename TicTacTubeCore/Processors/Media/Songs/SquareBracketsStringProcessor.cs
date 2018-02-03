using System;
using System.Linq;
using System.Text.RegularExpressions;
using TicTacTubeCore.Utils.Extensions.Strings;

namespace TicTacTubeCore.Processors.Media.Songs
{
	/// <summary>
	///     A string processor that can decide whether to keep or remove a text inside a square bracket.
	/// </summary>
	public class SquareBracketsStringProcessor : IStringProcessor
	{
		/// <summary>
		///     Square brackets that will be kept if the text contains one of these strings.
		/// </summary>
		protected string[] KeepIfContains = { };

		/// <summary>
		///     Square brackets that will be kept if the text ends with one of these strings.
		/// </summary>
		protected string[] KeepIfEnding = { "mashup", "remix", "release" };

		/// <summary>
		///     Square brackets that will be kept if the text starts with one of these strings.
		/// </summary>
		protected string[] KeepIfStarts = { };

		/// <summary>
		///     A regex that matches square brackets.
		/// </summary>
		protected Regex SquareBracketRegex = new Regex(@"\[([^\]]*?)\]");

		/// <inheritdoc />
		public string Process(string str)
		{
			var matches = SquareBracketRegex.Matches(str);

			foreach (Match match in matches)
			{
				string nestedText = match.Groups[1].Value;

				if (KeepIfEnding.Count(ending => nestedText.EndsWith(ending, StringComparison.OrdinalIgnoreCase)) <= 0 &&
				    KeepIfStarts.Count(start => nestedText.StartsWith(start, StringComparison.OrdinalIgnoreCase)) <= 0 &&
				    KeepIfContains.Count(contains => nestedText.Contains(contains, StringComparison.OrdinalIgnoreCase)) <= 0)
					str = str.Replace(match.Value, "");
			}

			return str;
		}
	}
}