using System;
using System.Threading;
using TicTacTubeCore.Executors.Events;
using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Sources.Files;
using TicTacTubeTest.Schedulers;

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
			var resetEvent = new ManualResetEventSlim();
			Scheduler.Executor.LifeCycleEvent += Unlock;

			Execute(source);

			if (resetEvent.Wait(timeOut))
			{
				Scheduler.Executor.LifeCycleEvent -= Unlock;
			}
			else
			{
				throw new TimeoutException("The file source has not been processed in the defined amount of time.");
			}

			void Unlock(object sender, ExecutorLifeCycleEventArgs args)
			{
				if (args.EventType == ExecutorLifeCycleEventType.SourceExecutionFinished &&
				    ReferenceEquals(args.FileSource, source))
				{
					resetEvent.Set();
				}
			}
		}
	}
}