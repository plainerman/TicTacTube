namespace TicTacTubeCore.Schedulers.Exceptions
{
	/// <summary>
	///     An exception that is thrown if an error occurs due to the state of the scheduler.
	/// </summary>
	public class SchedulerStateException : SchedulerException
	{
		/// <summary>
		///     Create a <see cref="SchedulerStateException" /> with a given message.
		/// </summary>
		/// <param name="message">The message (i.e. cause) of the exception.</param>
		public SchedulerStateException(string message) : base(message)
		{
		}
	}
}