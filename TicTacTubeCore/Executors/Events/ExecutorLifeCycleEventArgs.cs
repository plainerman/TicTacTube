using System;
using System.ComponentModel;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Executors.Events
{
	/// <summary>
	///     A lifecycle type / method.
	/// </summary>
	public enum ExecutorLifeCycleEventType
	{
		/// <summary>
		///     Once the executor has been initialized.
		/// </summary>
		Initialize,
		
		/// <summary>
		///		Once a new source will be added.
		/// </summary>
		SourceAdded,

		/// <summary>
		///		Once a given source begins to be processed.
		/// </summary>
		SourceExecutionStart,

		/// <summary>
		/// Once a given source finished execution.
		/// </summary>
		SourceExecutionFinished,

		/// <summary>
		///     When a scheduler stops scheduling.
		/// </summary>
		Stop
	}

	/// <inheritdoc />
	/// <summary>
	///     The <see cref="T:System.EventArgs" /> for events related to the lifecycle of the executor.
	/// </summary>
	public class ExecutorLifeCycleEventArgs : EventArgs
	{
		/// <summary>
		/// The type of the event that has been triggered.
		/// </summary>
		public ExecutorLifeCycleEventType EventType { get; }
		
		/// <summary>
		/// An optional (may be <code>null</code>) <see cref="IFileSource"/> containing a reference to the source.
		/// </summary>
		public IFileSource FileSource { get; }

		/// <summary>
		/// Create a new container for the information about a given executor lifecycle event.
		/// </summary>
		/// <param name="eventType">The type of the event that has been triggered.</param>
		/// <param name="fileSource">The file source related to this event. May be <code>null</code>.</param>
		public ExecutorLifeCycleEventArgs(ExecutorLifeCycleEventType eventType, IFileSource fileSource = null)
		{
			if (!Enum.IsDefined(typeof(ExecutorLifeCycleEventType), eventType))
				throw new InvalidEnumArgumentException(nameof(eventType), (int) eventType,
					typeof(ExecutorLifeCycleEventType));

			EventType = eventType;
			FileSource = fileSource;
		}
	}
}