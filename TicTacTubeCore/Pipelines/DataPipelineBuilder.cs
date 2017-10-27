namespace TicTacTubeCore.Pipelines
{
	/// <summary>
	///     A pipeline builder that builds a <see cref="IDataPipeline" />.
	/// </summary>
	public class DataPipelineBuilder : BaseDataPipelineBuilder
	{
		public override IDataPipeline Build()
		{
			return new DataPipeline(InternalPipeline);
		}
	}
}