using System;
using System.ComponentModel;
using TicTacTubeCore.Sources.Files;

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
		///		Once a given source is ready to be processed by an executor.
		/// </summary>
		SourceReady,

		/// <summary>
		///		If the wait condition of a source throws an exception, it will be discarded.
		/// </summary>
		SourceDiscarded,

		/// <summary>
		///     When a scheduler stops scheduling.
		/// </summary>
		Stop
	}

	/// <inheritdoc />
	/// <summary>
	///     The <see cref="T:System.EventArgs" /> for events related to the lifecycle of the scheduler.
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
		/// The source that this lifecycle type concerns. This may be <code>null</code>
		/// for <see cref="SchedulerLifeCycleEventType"/>s that do not process on a source.
		/// </summary>
		public IFileSource Source { get; }

		/// <summary>
		///     Create given event args with the current running state and the event type.
		/// </summary>
		/// <param name="isRunning">The running state. Determines whether the scheduler is currently active or not.</param>
		/// <param name="source">The source that is processed by this event. May be <code>null</code> for <see cref="SchedulerLifeCycleEventType"/>s that do not process on a source.</param>
		/// <param name="eventType">The current lifecycle type (i.e. method that caused the event).</param>
		public SchedulerLifeCycleEventArgs(bool isRunning, IFileSource source, SchedulerLifeCycleEventType eventType)
		{
			if (!Enum.IsDefined(typeof(SchedulerLifeCycleEventType), eventType))
				throw new InvalidEnumArgumentException(nameof(eventType), (int) eventType, typeof(SchedulerLifeCycleEventType));

			IsRunning = isRunning;
			EventType = eventType;
			Source = source;
		}
	}
}