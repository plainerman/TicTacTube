using System;

namespace TicTacTubeCore.Utils.Exceptions
{
	/// <summary>
	/// An exception that is thrown if an error occurs in the TicTacTube application itself.
	/// </summary>
	public abstract class TicTacException : Exception
	{
		/// <summary>
		/// Create a <see cref="TicTacException"/> with a given message.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		protected TicTacException(string message) : base(message)
		{
		}

		/// <summary>
		/// Create a <see cref="TicTacException"/> with a given message and an inner exception.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		/// <param name="innerException">The inner (causing) exception of this exception.</param>
		protected TicTacException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}