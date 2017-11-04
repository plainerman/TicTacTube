using System;
using TicTacTubeCore.Utils.Exceptions;

namespace TicTacTubeCore.Schedulers.Exceptions
{
	/// <summary>
	///     An exception that is thrown if an error occurs in the scheduler.
	/// </summary>
	public class SchedulerException : TicTacException
	{
		/// <summary>
		///     Create a <see cref="SchedulerException" /> with a given message.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		public SchedulerException(string message) : base(message)
		{
		}

		/// <summary>
		///     Create a <see cref="SchedulerException" /> with a given message and an inner exception.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		/// <param name="innerException">The inner (causing) exception of this exception.</param>
		public SchedulerException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}