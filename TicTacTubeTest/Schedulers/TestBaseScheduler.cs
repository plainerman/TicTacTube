using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Schedulers;
using TicTacTubeCore.Schedulers.Events;
using TicTacTubeTest.Sources.Files;
using TicTacTubeTest.Utils;

namespace TicTacTubeTest.Schedulers
{
	[TestClass]
	public class TestBaseScheduler
	{
		[TestMethod]
		public void TestLifeCycleEvents()
		{
			var scheduler = StartScheduler(out var collector);

			scheduler.Execute(new MockFileSource());
			scheduler.Execute(new MockFileSource());

			StopScheduler(scheduler, collector);

			Assert.AreEqual(4, collector.Events.Count, "Not all events have been stored");
			Assert.AreEqual(1, collector.Events.Count(e => e.EventType == SchedulerLifeCycleEventType.Start),
				"The number of start events has to be 1.");
			Assert.AreEqual(1, collector.Events.Count(e => e.EventType == SchedulerLifeCycleEventType.Stop),
				"The number of stop events has to be 1.");
			Assert.AreEqual(2, collector.Events.Count(e => e.EventType == SchedulerLifeCycleEventType.SourceReady),
				"The number of source ready events has to be 2.");
		}


		[TestMethod]
		public void TestDiscardSource()
		{
			var scheduler = StartScheduler(out var collector);

			scheduler.Execute(new MockFileSource(), s => throw new Exception("discard"));

			StopScheduler(scheduler, collector);

			Assert.IsTrue(collector.Events.Any(s => s.EventType == SchedulerLifeCycleEventType.SourceDiscarded),
				"No discarded source has been found.");
		}

		[TestMethod]
		public void TestDelayedSource()
		{
			var scheduler = StartScheduler(out var collector);
			int count = 0;

			scheduler.Execute(new MockFileSource(), s => count++ > 1);

			StopScheduler(scheduler, collector);

			Assert.IsTrue(collector.Events.Any(s => s.EventType == SchedulerLifeCycleEventType.SourceReady),
				"The added source never became ready.");
		}

		[TestMethod]
		public void TestDiscardNeverReadySourceWithStop()
		{
			var scheduler = StartScheduler(out var collector);
			scheduler.StopRetryCount = 1;

			scheduler.Execute(new MockFileSource(), s => false);

			StopScheduler(scheduler, collector);

			Assert.IsTrue(collector.Events.Any(s => s.EventType == SchedulerLifeCycleEventType.SourceDiscarded),
				"The added source was never discarded.");
			Assert.IsFalse(collector.Events.Any(s => s.EventType == SchedulerLifeCycleEventType.SourceReady),
				"The added source became ready.");
		}

		private static BaseSchedulerImpl StartScheduler(out EventCollector<SchedulerLifeCycleEventArgs> collector)
		{
			var scheduler = new BaseSchedulerImpl() { SourceConditionDelay = 200 };
			collector = new EventCollector<SchedulerLifeCycleEventArgs>();

			scheduler.LifeCycleEvent += collector.Collect;

			scheduler.Start();
			return scheduler;
		}

		private static void StopScheduler(IScheduler scheduler, EventCollector<SchedulerLifeCycleEventArgs> collector)
		{
			scheduler.Stop();
			scheduler.Join();

			scheduler.LifeCycleEvent -= collector.Collect;
		}
	}
}