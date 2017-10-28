using System;

namespace TicTacTubeCore.Schedulers
{
	/// <summary>
	///     A scheduler that can be executed by manually calling a method.
	/// </summary>
	public class EventFiringScheduler : BaseScheduler
	{
		/// <summary>
		/// Execute the start of the scheduler.
		/// </summary>
		protected override void ExecuteStart()
		{
		}

		/// <summary>
		/// Execute the stop of the scheduler.
		/// </summary>
		protected override void ExecuteStop()
		{
		}

		/// <summary>
		/// Force the scheduler to execute the pipeline.
		/// </summary>
		public virtual void Fire()
		{
			Execute();
		}

		/// <summary>
		/// A method that ignores the parmeters but is handy for adding it to an external event.
		/// </summary>
		/// <param name="sender">Ignored.</param>
		/// <param name="args">Ignored.</param>
		public virtual void Fire(object sender, EventArgs args)
		{
			Fire();
		}
	}
}