namespace TicTacTubeCore.Pipelines.Exceptions
{
	/// <summary>
	/// An exception that is thrown if an error occurs due to the state of the pipeline.
	/// </summary>
	public class PipelineStateException : PipelineException
	{
		/// <summary>
		/// Create a <see cref="PipelineStateException"/> with a given message.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		public PipelineStateException(string message) : base(message)
		{
		}
	}
}