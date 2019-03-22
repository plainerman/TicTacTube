using System;
using System.Threading;
using TicTacTubeCore.Executors.Events;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Sources.Files;
using TicTacTubeTest.Schedulers;
using TicTacTubeTest.Utils.Extensions.Schedulers;

namespace TicTacTubeTest.Processors
{
	public class SimpleTestScheduler
	{
		public readonly IDataPipelineBuilder Builder;
		public readonly BaseSchedulerImpl Scheduler;

		public SimpleTestScheduler()
		{
			Scheduler = new BaseSchedulerImpl();
			Builder = new DataPipelineBuilder();
			Scheduler.Add(Builder);
		}

		public virtual void Execute(IFileSource source)
		{
			Scheduler.Execute(source);
		}

		public virtual void Start()
		{
			Scheduler.Start();
		}

		public virtual void ExecuteBlocking(IFileSource source, int timeOut = 5000)
		{
			Scheduler.ExecuteBlocking(source, timeOut);
		}
	}
}