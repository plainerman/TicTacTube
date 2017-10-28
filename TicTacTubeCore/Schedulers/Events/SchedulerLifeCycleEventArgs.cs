using System;
using System.ComponentModel;

namespace TicTacTubeCore.Schedulers.Events
{
	public enum SchedulerLifeCycleEventType
	{
		Start,
		Execute,
		Stop
	}

	public class SchedulerLifeCycleEventArgs : SchedulerEventArgs
	{
		public SchedulerLifeCycleEventArgs(bool isRunning, SchedulerLifeCycleEventType eventType)
		{
			if (!Enum.IsDefined(typeof(SchedulerLifeCycleEventType), eventType))
				throw new InvalidEnumArgumentException(nameof(eventType), (int)eventType, typeof(SchedulerLifeCycleEventType));

			IsRunning = isRunning;
			EventType = eventType;
		}

		public bool IsRunning { get; }
		public SchedulerLifeCycleEventType EventType { get; }
	}
}