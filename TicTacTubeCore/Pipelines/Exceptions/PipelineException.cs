using System;
using TicTacTubeCore.Utils.Exceptions;

namespace TicTacTubeCore.Pipelines.Exceptions
{
	public class PipelineException : TicTacException
	{
		public PipelineException(string message) : base(message)
		{
		}

		public PipelineException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}