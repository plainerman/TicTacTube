using System;
using System.ComponentModel;
using TicTacTubeCore.Pipelines;
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
		///		If an exception is thrown during the execution of the source.
		/// </summary>
		SourceExecutionFailed,

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
		/// The pipeline in which a given error (see <see cref="Error"/>) was thrown.
		/// This is <code>null</code>, except it is a <see cref="ExecutorLifeCycleEventType.SourceExecutionFailed"/> event.
		/// </summary>
		public IDataPipelineOrBuilder Pipeline { get; }

		/// <summary>
		/// The error that was thrown in a given pipeline (see <see cref="Pipeline"/>).
		/// This is <code>null</code>, except it is a <see cref="ExecutorLifeCycleEventType.SourceExecutionFailed"/> event.
		/// </summary>
		public Exception Error { get; }

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

		/// <inheritdoc />
		/// <summary>
		/// Create a new <see cref="F:TicTacTubeCore.Executors.Events.ExecutorLifeCycleEventType.SourceExecutionFailed" /> event args caused by a given exception.
		/// </summary>
		/// <param name="pipeline">The pipeline in which the error was thrown.</param>
		/// <param name="fileSource">The <see cref="T:TicTacTubeCore.Sources.Files.IFileSource" /> that caused the exception.</param>
		/// <param name="error">The exception that was thrown inside the pipeline.</param>
		public ExecutorLifeCycleEventArgs(IDataPipelineOrBuilder pipeline, IFileSource fileSource, Exception error) : this(ExecutorLifeCycleEventType.SourceExecutionFailed, fileSource)
		{
			Pipeline = pipeline;
			Error = error;
		}
	}
}