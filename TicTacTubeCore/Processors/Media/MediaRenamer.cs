using System;
using TicTacTubeCore.Processors.Filesystem;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Media
{
	/// <summary>
	///     A source renamer that is optimized for renaming media files based on extracted media info.
	/// </summary>
	public class MediaRenamer<T> : SourceRenamer where T : IMediaInfo
	{
		/// <summary>
		///     The media info extractor that is used to correctly extract the media info.
		/// </summary>
		protected readonly IMediaInfoExtractor<T> MediaInfoExtractor;

		/// <summary>
		///     The name generator that generates the names for a given <see cref="IMediaInfo" />.
		/// </summary>
		protected readonly IMediaNameGenerator<T> NameGenerator;

		/// <summary>
		///     Create a new media renamer that uses a <see cref="IMediaNameGenerator{T}" /> to generate a filename (without
		///     extension)
		///     based on the media info extracted by the <see cref="IMediaInfoExtractor{T}" />.
		/// </summary>
		/// <param name="nameGenerator">The name generator that generates the new file names.</param>
		/// <param name="extractor">The extractor that extracts a media info from the file source.</param>
		/// <param name="override"><c>True</c>, if an existing destination should be overriden.</param>
		public MediaRenamer(IMediaNameGenerator<T> nameGenerator, IMediaInfoExtractor<T> extractor, bool @override = true) :
			base(@override)
		{
			NameGenerator = nameGenerator ?? throw new ArgumentNullException(nameof(nameGenerator));
			MediaInfoExtractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
			NameProducer = ProduceName;
		}

		/// <summary>
		///     Create a new media renamer that uses a <see cref="IMediaNameGenerator{T}" /> to generate a filename (without
		///     extension)
		///     based on the media info extracted by the <see cref="IMediaInfoExtractor{T}" />.
		///     A <see cref="PatternMediaNameGenerator{T}" /> is used as <see cref="IMediaNameGenerator{T}" />.
		/// </summary>
		/// <param name="pattern">
		///     The pattern that is used to generate filenames. See <see cref="PatternMediaNameGenerator{T}" />
		///     for more information..
		/// </param>
		/// <param name="extractor">The extractor that extracts a media info from the file source.</param>
		/// <param name="override"><c>True</c>, if an existing destination should be overriden.</param>
		public MediaRenamer(string pattern, IMediaInfoExtractor<T> extractor, bool @override = true) : this(
			new PatternMediaNameGenerator<T>(pattern), extractor, @override)
		{
		}

		/// <summary>
		///     This method will be assigned as name producer to the renamer.
		///     Per default, it simple uses the media info extractor and name generator.
		/// </summary>
		/// <param name="source">The file source that will be parsed.</param>
		/// <returns>The new filename for the file source (with the file extension).</returns>
		protected virtual string ProduceName(IFileSource source) => NameGenerator.Parse(MediaInfoExtractor.Extract(source)) +
		                                                            source.FileExtension;
	}
}