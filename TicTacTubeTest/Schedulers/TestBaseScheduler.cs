using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TagLib.Riff;
using TicTacTubeCore.Schedulers.Events;
using TicTacTubeCore.Sources.Files;
using TicTacTubeTest.Sources.Files;

namespace TicTacTubeTest.Schedulers
{
	[TestClass]
	public class TestBaseScheduler
	{
		[TestMethod]
		public void TestLifeCycleEvents()
		{
			var scheduler = new BaseSchedulerImpl();
			var events = new List<SchedulerLifeCycleEventArgs>();

			scheduler.LifeCycleEvent += (o, a) => events.Add(a);

			scheduler.Start();

			scheduler.Execute(new MockFileSource());
			scheduler.Execute(new MockFileSource());

			scheduler.Stop();
			scheduler.Join();

			Assert.AreEqual(4, events.Count, "Not all events have been stored");
			Assert.AreEqual(1, events.Count(e => e.EventType == SchedulerLifeCycleEventType.Start), "The number of start events has to be 1.");
			Assert.AreEqual(1, events.Count(e => e.EventType == SchedulerLifeCycleEventType.Stop), "The number of stop events has to be 1.");
			Assert.AreEqual(2, events.Count(e => e.EventType == SchedulerLifeCycleEventType.SourceReady), "The number of source ready events has to be 2.");
		}
	}
}