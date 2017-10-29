using System;
using System.ComponentModel;

namespace TicTacTubeCore.Schedulers.Events
{
	/// <summary>
	///     A lifecycle type / method.
	/// </summary>
	public enum SchedulerLifeCycleEventType
	{
		/// <summary>
		///     When a scheduler starts scheduling.
		/// </summary>
		Start,

		/// <summary>
		///     When a scheduler executes the pipeline.
		/// </summary>
		Execute,

		/// <summary>
		///     When a scheduler stops scheduling.
		/// </summary>
		Stop
	}

	/// <summary>
	///     The <see cref="EventArgs" /> for events related to the lifecycle of the scheduler.
	/// </summary>
	public class SchedulerLifeCycleEventArgs : SchedulerEventArgs
	{
		/// <summary>
		///     The running state.Determines whether the scheduler is currently active or not.
		/// </summary>
		public bool IsRunning { get; }

		/// <summary>
		///     The current lifecycle type (i.e. method that caused the event).
		/// </summary>
		public SchedulerLifeCycleEventType EventType { get; }

		/// <summary>
		///     Create given event args with the current running state and the event type.
		/// </summary>
		/// <param name="isRunning">The running state. Determines whether the scheduler is currently active or not.</param>
		/// <param name="eventType">The current lifecycle type (i.e. method that caused the event).</param>
		public SchedulerLifeCycleEventArgs(bool isRunning, SchedulerLifeCycleEventType eventType)
		{
			if (!Enum.IsDefined(typeof(SchedulerLifeCycleEventType), eventType))
			{
				throw new InvalidEnumArgumentException(nameof(eventType), (int) eventType, typeof(SchedulerLifeCycleEventType));
			}

			IsRunning = isRunning;
			EventType = eventType;
		}
	}
}