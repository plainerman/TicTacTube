using System;

namespace TicTacTubeCore.Schedulers
{
	/// <summary>
	///     A scheduler that can be executed by manually calling a method.
	/// </summary>
	public class EventFiringScheduler : BaseScheduler
	{
		protected override void ExecuteStart()
		{
		}

		protected override void ExecuteStop()
		{
		}

		public virtual void Fire(object sender, EventArgs args)
		{
			Execute();
		}
	}
}