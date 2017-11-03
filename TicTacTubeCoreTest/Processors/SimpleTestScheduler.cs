using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCoreTest.Schedulers;

namespace TicTacTubeCoreTest.Processors
{
	public class SimpleTestScheduler
	{
		private readonly BaseSchedulerImpl _scheduler;
		public readonly IDataPipelineBuilder Builder;

		public SimpleTestScheduler()
		{
			_scheduler = new BaseSchedulerImpl();
			Builder = new DataPipelineBuilder();
			_scheduler.Add(Builder);
		}

		public virtual void Execute(IFileSource source)
		{
			_scheduler.Execute(source);
		}
	}
}