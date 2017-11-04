using System.IO;
using TicTacTubeCore.Sources.Files.External;

namespace TicTacTubeCoreTest.Sources.Files
{
	public class TempExternalFileSource : BaseExternalFileSource
	{
		public const string DummyText =
			"This is a temporary file for testing purposes. Only delete if no test is running";

		public TempExternalFileSource(bool lazyLoading) : base(lazyLoading)
		{
		}

		/// <summary>
		///     This method returns a full path (i.e. a file). This file does not exist yet and can be created.
		/// </summary>
		/// <param name="path">The path to the folder where the file should be created.</param>
		/// <returns>The full path to a possible file.</returns>
		protected virtual string GetTempFileFromPath(string path)
		{
			const string baseFilename = ".tictactemp";

			if (!File.Exists(Path.Combine(path, baseFilename)))
			{
				return Path.Combine(path, baseFilename);
			}

			int index = 0;

			while (true)
			{
				string fileName = Path.Combine(path, $"{baseFilename}-{index}");

				if (!File.Exists(fileName))
				{
					return fileName;
				}

				index++;
			}
		}

		/// <inheritdoc />
		protected override void Download(string destinationPath)
		{
			FinishedPath = GetTempFileFromPath(destinationPath);
			File.WriteAllText(FinishedPath, DummyText);
		}

		/// <inheritdoc />
		protected override void DownloadAsync(string destinationPath)
		{
			FinishedPath = GetTempFileFromPath(destinationPath);
			CurrentDownloadTask = File.WriteAllTextAsync(FinishedPath, DummyText);
		}
	}
}