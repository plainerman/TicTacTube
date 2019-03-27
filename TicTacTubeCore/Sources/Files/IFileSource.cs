using System.IO;

namespace TicTacTubeCore.Sources.Files
{
	/// <inheritdoc />
	/// <summary>
	///     A <see cref="IDataSource"/> that is simply pointing to a file.
	///		This is the most commonly used <see cref="IDataSource"/>.
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

		/// <summary>
		///     This method will be called before executing this source for the first time.
		/// </summary>
		void Init();

		/// <summary>
		///     This method will be called before executing a data processor.
		/// </summary>
		void BeginExecute();

		/// <summary>
		///     This method will be called after executing a data processor.
		/// </summary>
		void EndExecute();
	}
}