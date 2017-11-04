using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Pipelines
{
	/// <summary>
	///     A pipeline that stores multiple processors that process / modify / ... some sort of data.
	/// </summary>
	public interface IDataPipeline : IDataPipelineOrBuilder
	{
		/// <summary>
		///     Actually execute the complete pipeline
		/// </summary>
		/// <param name="fileSource">The filesource this pipeline is executed on.</param>
		void Execute(IFileSource fileSource);
	}
}