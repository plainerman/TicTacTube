using System;
using TicTacTubeCore.Utils.Exceptions;

namespace TicTacTubeCore.Soundcloud.Processors.Media.Songs.Exceptions
{
	/// <summary>
	///     An exception prototype for all exceptions related to soundcloud.
	/// </summary>
	public abstract class SoundCloudException : TicTacException
	{
		/// <summary>
		///     Create a <see cref="SoundCloudException" /> with a given message.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		protected SoundCloudException(string message) : base(message)
		{
		}

		/// <summary>
		///     Create a <see cref="SoundCloudException" /> with a given message and an inner exception.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		/// <param name="innerException">The inner (causing) exception of this exception.</param>
		protected SoundCloudException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}