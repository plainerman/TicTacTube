namespace TicTacTubeCore.Pipelines
{
	/// <summary>
	///     A data pipeline (a set of multiple data processors), or the builder for the pipeline.
	/// </summary>
	public interface IDataPipelineOrBuilder
	{
		/// <summary>
		///     Check whether the object is a builder or not.
		/// </summary>
		bool IsBuilder { get; }

		/// <summary>
		///     This method returns either a newly built data pipeline (if its a builder), or
		///     the same data pipeline.
		/// </summary>
		/// <returns>
		///     This method returns either a newly built data pipeline (if its a builder), or
		///     the same data pipeline.
		/// </returns>
		IDataPipeline Build();
	}
}