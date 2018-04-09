using TicTacTubeCore.Schedulers;
using TicTacTubeCore.Sources.Files;

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

		public new void Execute(IFileSource fileSource)
		{
			base.Execute(fileSource);
		}
	}
}