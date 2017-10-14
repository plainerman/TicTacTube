namespace TicTacCore.Sources.Files
{
	public interface IFileSource : IDataSource
	{
		string FileName { get; set; }
		string FileExtension { get; set; }
		string Path { get; set; }
	}
}