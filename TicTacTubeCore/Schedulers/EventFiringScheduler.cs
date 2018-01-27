using System;
using TicTacTubeCore.Schedulers.Exceptions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Schedulers
{
	/// <summary>
	///     A scheduler that can be executed by manually calling a method.
	/// </summary>
	public class EventFiringScheduler : BaseScheduler
	{
		/// <summary>
		///     Execute the start of the scheduler.
		/// </summary>
		protected override void ExecuteStart()
		{
		}

		/// <summary>
		///     Execute the stop of the scheduler.
		/// </summary>
		protected override void ExecuteStop()
		{
		}

		/// <summary>
		///     Force the scheduler to execute the pipeline.
		/// </summary>
		/// <param name="fileSource">The file source that will be processed.</param>
		/// <exception cref="SchedulerStateException">
		///     If the scheduler is not running. Call <see cref="BaseScheduler.Start" />
		/// </exception>
		public virtual void Fire(IFileSource fileSource)
		{
			Execute(fileSource);
		}

		/// <summary>
		///     A method that ignores the parmeters but is handy for adding it to an external event.
		/// </summary>
		/// <param name="sender">Ignored.</param>
		/// <param name="args">Ignored.</param>
		/// <param name="fileSource">The file source that will be processed.</param>
		public virtual void Fire(object sender, EventArgs args, IFileSource fileSource)
		{
			Fire(fileSource);
		}
	}
}