using TicTacTubeCore.Executors;
using TicTacTubeCore.Schedulers;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeTest.Schedulers
{
	public class BaseSchedulerImpl : BaseScheduler
	{
		public BaseSchedulerImpl(IExecutor executor = null) : base(executor)
		{
		}

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