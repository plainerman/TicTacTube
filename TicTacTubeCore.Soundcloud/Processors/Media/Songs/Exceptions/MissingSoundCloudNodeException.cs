using System;
using TicTacTubeCore.Utils.Exceptions;

namespace TicTacTubeCore.Soundcloud.Processors.Media.Songs.Exceptions
{
	/// <summary>
	///     An exception prototype for all exceptions related to a missing node or element in the webpage of a soundcloud url.
	/// </summary>
	public abstract class MissingSoundCloudNodeException : TicTacException
	{
		/// <summary>
		///     Create a <see cref="MissingSoundCloudNodeException" /> with a given message.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		protected MissingSoundCloudNodeException(string message) : base(message)
		{
		}

		/// <summary>
		///     Create a <see cref="MissingSoundCloudNodeException" /> with a given message and an inner exception.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		/// <param name="innerException">The inner (causing) exception of this exception.</param>
		protected MissingSoundCloudNodeException(string message, Exception innerException) : base(message,
			innerException)
		{
		}
	}
}