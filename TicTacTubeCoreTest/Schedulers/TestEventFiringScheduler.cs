using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Schedulers;
using TicTacTubeCore.Schedulers.Events;

namespace TicTacTubeCoreTest.Schedulers
{
	[TestClass]
	public class TestEventFiringScheduler
	{
		private event EventHandler TestEvent;

		[TestMethod]
		public void TestExecute()
		{
			int executeCounter = 0;

			var eventFiringScheduler = new EventFiringScheduler();
			eventFiringScheduler.LifeCycleEvent += Executed;

			eventFiringScheduler.Start();

			TestEvent += EmptyFire;

			Assert.AreEqual(0, executeCounter);

			TestEvent?.Invoke(this, EventArgs.Empty);
			Assert.AreEqual(1, executeCounter);

			TestEvent?.Invoke(this, EventArgs.Empty);
			Assert.AreEqual(2, executeCounter);

			eventFiringScheduler.Stop();

			Assert.AreEqual(2, executeCounter);

			TestEvent -= EmptyFire;

			void Executed(object sender, SchedulerLifeCycleEventArgs args)
			{
				if (args.EventType == SchedulerLifeCycleEventType.Execute)
					executeCounter++;
			}

			void EmptyFire(object sender, EventArgs args)
			{
				eventFiringScheduler.Fire(sender, args, null);
			}
		}
	}
}