using System.IO;

namespace TicTacTubeCore.Sources.Files
{
	/// <summary>
	///     A datasource that is simply a file.
	/// </summary>
	public interface IFileSource : IDataSource
	{
		/// <summary>
		///     The file info containing all information of the file.
		/// </summary>
		FileInfo FileInfo { get; }

		/// <summary>
		///     The name of the file without extension.
		/// </summary>
		string FileName { get; }

		/// <summary>
		///     The name of the file with extension.
		/// </summary>
		string FullFileName { get; }

		/// <summary>
		///     The extension of the file.
		/// </summary>
		string FileExtension { get; }

		/// <summary>
		///     The path of the file.
		/// </summary>
		string Path { get; }
	}
}