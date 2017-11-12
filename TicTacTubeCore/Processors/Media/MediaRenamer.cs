using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TicTacTubeCore.Processors.Filesystem;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Utils.Extensions;

namespace TicTacTubeCore.Processors.Media
{
	/// <summary>
	/// A source renamer that is optimized for renaming media files based on extracted media info.
	/// </summary>
	public class MediaRenamer<T> : SourceRenamer where T : IMediaInfo
	{
		private static readonly Regex CurlyBracketsMatcher = new Regex("{(.*?)}");

		protected readonly IMediaInfoExtractor<T> MediaInfoExtractor;

		protected readonly string Pattern;

		protected string[] VariableNames;

		public MediaRenamer(string pattern, IMediaInfoExtractor<T> extractor)
		{
			if (pattern == null) throw new ArgumentNullException(nameof(pattern));

			MediaInfoExtractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
			NameProducer = ProduceName;

			Pattern = PreparePattern(pattern);
		}

		protected string PreparePattern(string pattern)
		{
			int openCount = pattern.Count(f => f == '{');
			int closeCount = pattern.Count(f => f == '}');

			if (openCount > closeCount) throw new ArgumentException("Bad formatting - } missing.", nameof(pattern));
			if (openCount < closeCount) throw new ArgumentException("Bad formatting - { missing.", nameof(pattern));

			var matches = CurlyBracketsMatcher.Matches(pattern);
			VariableNames = new string[matches.Count];

			int offset = 0;

			for (int i = 0; i < matches.Count; i++)
			{
				VariableNames[i] = matches[i].Groups[1].Value;
				if (typeof(T).GetField(VariableNames[i], BindingFlags.Public | BindingFlags.Instance) == null)
					throw new FormatException($"Unknown parameter \"{VariableNames[i]}\"");

				string replace = i.ToString();
				pattern = pattern.ReplaceFirst(VariableNames[i], replace, matches[i].Index + offset);

				offset -= VariableNames[i].Length - replace.Length;
			}

			return pattern;
		}

		protected string ProduceName(IFileSource source) => ParsePattern(MediaInfoExtractor.Extract(source));

		protected virtual string ParsePattern(T info)
		{
			throw new NotImplementedException();
		}
	}
}