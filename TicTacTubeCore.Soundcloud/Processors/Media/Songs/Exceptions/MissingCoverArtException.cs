using System;

namespace TicTacTubeCore.Soundcloud.Processors.Media.Songs.Exceptions
{
	/// <summary>
	///     An exception that is thrown if a cover art is missing on a soundcloud page.
	/// </summary>
	public class MissingCoverArtException : MissingSoundCloudNodeException
	{
		/// <summary>
		///     Create a <see cref="MissingCoverArtException" /> with a given message.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		public MissingCoverArtException(string message) : base(message)
		{
		}

		/// <summary>
		///     Create a <see cref="MissingCoverArtException" /> with a given message and an inner exception.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		/// <param name="innerException">The inner (causing) exception of this exception.</param>
		public MissingCoverArtException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}