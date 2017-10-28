using TicTacTubeCore.Schedulers;

namespace TicTacTubeCoreTest.Schedulers
{
	public class BaseSchedulerImpl : BaseScheduler
	{
		protected override void ExecuteStart()
		{
		}

		protected override void ExecuteStop()
		{
		}

		public new void Execute()
		{
			base.Execute();
		}
	}
}