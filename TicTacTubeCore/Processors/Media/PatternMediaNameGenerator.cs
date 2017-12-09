using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TicTacTubeCore.Processors.Media.Songs;
using TicTacTubeCore.Utils.Extensions.Strings;

namespace TicTacTubeCore.Processors.Media
{
	/// <summary>
	///     A <see cref="IMediaNameGenerator{T}" /> that works by using patterns and the corresponding file names.
	/// </summary>
	public class PatternMediaNameGenerator<T> : IMediaNameGenerator<T> where T : IMediaInfo
	{
		/// <summary>
		///     The bracket that is used to begin a variable reference in a pattern.
		/// </summary>
		public const char OpenBracket = '{';

		/// <summary>
		///     The bracket that is used to end a variable reference in a pattern.
		/// </summary>
		public const char CloseBracket = '}';

		/// <summary>
		///     A simple regex matcher is used to find curly brackets with text inside them.
		/// </summary>
		protected readonly Regex CurlyBracketsMatcher = new Regex("{(.*?)}");

		/// <summary>
		///     The mapped pattern that defines how to rename files. E.g. {0} file -> SomeVar file
		/// </summary>
		protected readonly string Pattern;

		/// <summary>
		///     The variable names that will be extracted from the media info.
		/// </summary>
		protected string[] VariableNames;

		/// <summary>
		///     Create a new media name generator that allows you to simply access variables from the <see cref="IMediaInfo" />.
		///     E.g. <see cref="SongInfo" />
		///     The pattern "{Album} - {Title}" will fill in the album and title from the given <see cref="SongInfo" />.
		/// </summary>
		/// <param name="pattern">The pattern that is used to generate names.</param>
		public PatternMediaNameGenerator(string pattern)
		{
			Pattern = PreparePattern(pattern);
		}

		/// <inheritdoc />
		public string Parse(T info)
		{
			string name = Pattern;

			for (int i = 0; i < VariableNames.Length; i++)
			{
				var current = typeof(T).GetField(VariableNames[i], BindingFlags.Public | BindingFlags.Instance).GetValue(info);

				string currentAsString;
				if (current is Array)
					currentAsString = string.Join(", ", ((IEnumerable) current).Cast<object>().Select(o => o.ToString()));
				else
					currentAsString = current.ToString();
				//= current is Array ? string.Join(", ", current) : current.ToString();

				name = name.Replace($"{OpenBracket}{i}{CloseBracket}", currentAsString);
			}

			return name;
		}

		/// <summary>
		///     Prepare a pattern with strings inside curly brackets, test the variable names, resolve them, and correctly set
		///     variable names.
		/// </summary>
		/// <param name="pattern">The base pattern.</param>
		/// <returns>The fully parsed pattern.</returns>
		protected string PreparePattern(string pattern)
		{
			int openCount = pattern.Count(f => f == OpenBracket);
			int closeCount = pattern.Count(f => f == CloseBracket);

			if (openCount > closeCount)
				throw new FormatException($"Bad formatting - {OpenBracket} missing.");
			if (openCount < closeCount)
				throw new FormatException($"Bad formatting - {CloseBracket} missing.");

			var matches = CurlyBracketsMatcher.Matches(pattern);
			var variableNames = new List<string>();

			int offset = 0;

			for (int i = 0; i < matches.Count; i++)
			{
				string currentVar = matches[i].Groups[1].Value;
				int currentVarIndex = i;

				if (variableNames.Contains(currentVar))
				{
					currentVarIndex = variableNames.IndexOf(currentVar);
				}
				else
				{
					variableNames.Add(currentVar);
					if (typeof(T).GetField(currentVar, BindingFlags.Public | BindingFlags.Instance) == null)
						throw new FormatException($"Unknown parameter \"{currentVar}\"");
				}

				string replace = currentVarIndex.ToString();
				pattern = pattern.ReplaceFirst(variableNames[currentVarIndex], replace, matches[i].Index + offset);

				offset -= currentVar.Length - replace.Length;
			}

			VariableNames = variableNames.ToArray();

			return pattern;
		}
	}
}