using TicTacTubeCore.Processors.Songs;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Media.Songs
{
	/// <summary>
	/// A simple song info extractor that tries as hard as it can.
	/// </summary>
	public class SongInfoExtractor : IMediaInfoExtractor<SongInfo>
	{
		/// <inheritdoc />
		public SongInfo Extract(IFileSource song)
		{
			//TODO: implement
			throw new System.NotImplementedException();
		}
	}
}