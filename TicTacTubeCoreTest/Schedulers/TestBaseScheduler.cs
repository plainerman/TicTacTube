using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTubeCore.Schedulers.Events;

namespace TicTacTubeCoreTest.Schedulers
{
	[TestClass]
	public class TestBaseScheduler
	{
		[TestMethod]
		public void TestLifeCycleEvents()
		{
			var scheduler = new BaseSchedulerImpl();
			bool desiredRunning = true;
			var desiredType = SchedulerLifeCycleEventType.Start;

			scheduler.LifeCycleEvent += (o, a) => TestArgs(a);

			scheduler.Start();

			desiredType = SchedulerLifeCycleEventType.Execute;

			scheduler.Execute();
			scheduler.Execute();

			desiredType = SchedulerLifeCycleEventType.Stop;
			desiredRunning = false;

			scheduler.Stop();

			void TestArgs(SchedulerLifeCycleEventArgs args)
			{
				Assert.AreEqual(desiredRunning, args.IsRunning);
				Assert.AreEqual(desiredType, args.EventType);
			}
		}
	}
}