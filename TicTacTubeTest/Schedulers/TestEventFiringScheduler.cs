using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Schedulers;
using TicTacTubeCore.Schedulers.Events;
using TicTacTubeTest.Sources.Files;

namespace TicTacTubeTest.Schedulers
{
	[TestClass]
	public class TestEventFiringScheduler
	{
		[TestMethod]
		public void TestExecute()
		{
			int executeCounter = 0;

			var eventFiringScheduler = new EventFiringScheduler();
			eventFiringScheduler.LifeCycleEvent += Executed;

			eventFiringScheduler.Start();

			eventFiringScheduler.Fire(new MockFileSource());
			eventFiringScheduler.Fire(new MockFileSource());
			eventFiringScheduler.Fire(null, EventArgs.Empty, new MockFileSource());

			eventFiringScheduler.Stop();
			eventFiringScheduler.Join();

			Assert.AreEqual(3, executeCounter);

			void Executed(object sender, SchedulerLifeCycleEventArgs args)
			{
				if (args.EventType == SchedulerLifeCycleEventType.SourceReady)
					executeCounter++;
			}
		}
	}
}