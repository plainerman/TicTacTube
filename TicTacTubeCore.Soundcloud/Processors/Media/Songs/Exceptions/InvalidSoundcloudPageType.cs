using System;

namespace TicTacTubeCore.Soundcloud.Processors.Media.Songs.Exceptions
{
	/// <summary>
	///     An exception that is thrown if this soundcloud page type is not supported.
	/// </summary>
	public class InvalidSoundcloudPageType : SoundCloudException
	{

		/// <summary>
		///     Create a <see cref="InvalidSoundcloudPageType" /> with a given message.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		public InvalidSoundcloudPageType(string message) : base(message) { }

		/// <summary>
		///     Create a <see cref="InvalidSoundcloudPageType" /> with a given message and an inner exception.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		/// <param name="innerException">The inner (causing) exception of this exception.</param>
		public InvalidSoundcloudPageType(string message, Exception innerException) : base(message, innerException) { }
	}
}