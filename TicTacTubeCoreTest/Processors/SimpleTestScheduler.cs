﻿using TicTacTubeCore.Pipelines;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCoreTest.Schedulers;

namespace TicTacTubeCoreTest.Processors
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
	}
}