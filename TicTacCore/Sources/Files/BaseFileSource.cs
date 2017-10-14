using SystemPath = System.IO.Path;

namespace TicTacCore.Sources.Files
{
	public abstract class BaseFileSource : IFileSource
	{
		public string FileName { get; set; }
		public string FileExtension { get; set; }
		public string Path { get; set; }

		protected BaseFileSource(string filePath)
		{
			Path = SystemPath.GetPathRoot(filePath);
			FileName = SystemPath.GetFileNameWithoutExtension(filePath);
			FileExtension = SystemPath.GetExtension(filePath);
		}
	}
}