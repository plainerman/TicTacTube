namespace TicTacTubeCore.Pipelines
{
	/// <summary>
	///     A pipeline builder that builds a <see cref="T:TicTacTubeCore.Pipelines.IDataPipeline" />.
	/// </summary>
	public class DataPipelineBuilder : BaseDataPipelineBuilder
	{
		/// <inheritdoc />
		public override IDataPipeline Build() => new DataPipeline(InternalPipeline);
	}
}