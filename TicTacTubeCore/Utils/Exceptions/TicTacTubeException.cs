using System;

namespace TicTacTubeCore.Utils.Exceptions
{
	public abstract class TicTacException : Exception
	{
		protected TicTacException(string message) : base(message)
		{
		}

		protected TicTacException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}