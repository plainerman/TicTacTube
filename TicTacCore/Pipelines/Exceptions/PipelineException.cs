using System;
using TicTacCore.Utils.Exceptions;

namespace TicTacCore.Pipelines.Exceptions
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