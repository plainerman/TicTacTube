using System.Reflection.Metadata.Ecma335;
using TicTacTubeCore.Sources.Files.External;

namespace TicTacTubeCore.Sources.Files
{
	/// <summary>
	///     The default implementation of a file source.
	/// </summary>
	public class FileSource : BaseFileSource
	{
		/// <summary>
		///     Create a <see cref="FileSource" /> from a given filepath. This cannot be changed.
		/// </summary>
		/// <param name="filePath">The absolute or relative filepath. This may not be <c>null</c> or empty.</param>
		public FileSource(string filePath) : base(filePath)
		{
		}

		/// <inheritdoc />
		public FileSource(IExternalFileSource externalSource, string localPath) : base(externalSource, localPath)
		{
		}

		/// <summary>
		/// Automatically create a new file source from a given string (i.e. the path to the file).
		/// </summary>
		/// <param name="path">The absolute or relative filepath. This may not be <c>null</c> or empty.</param>
		public static implicit operator FileSource(string path)
		{
			return new FileSource(path);
		}
	}
}