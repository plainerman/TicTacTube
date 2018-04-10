using System;

namespace TicTacTubeCore.Soundcloud.Processors.Media.Songs.Exceptions
{
	/// <summary>
	///     An exception that is thrown if the genre is missing on a soundcloud page.
	/// </summary>
	public class MissingGenreException : MissingSoundCloudNodeException
	{
		/// <summary>
		///     Create a <see cref="MissingGenreException" /> with a given message.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		public MissingGenreException(string message) : base(message)
		{
		}

		/// <summary>
		///     Create a <see cref="MissingGenreException" /> with a given message and an inner exception.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		/// <param name="innerException">The inner (causing) exception of this exception.</param>
		public MissingGenreException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}