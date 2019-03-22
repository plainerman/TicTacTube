﻿using System;
using TicTacTubeCore.Executors;
using TicTacTubeCore.Schedulers.Exceptions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Schedulers
{
	/// <inheritdoc />
	/// <summary>
	///     A scheduler that can be executed by manually calling a method.
	/// </summary>
	public class EventFiringScheduler : BaseScheduler
	{
		/// <inheritdoc />
		/// <summary>
		/// Create a new scheduler that can be triggered manually.
		/// </summary>
		/// <param name="executor">The executor that will be used. If <code>null</code>,
		/// the default executor specified in <see cref="T:TicTacTubeCore.Schedulers.BaseScheduler" /> will be used. </param>
		public EventFiringScheduler(IExecutor executor = null) : base(executor)
		{
		}

		/// <inheritdoc />
		/// <summary>
		///     Execute the start of the scheduler.
		/// </summary>
		protected override void ExecuteStart()
		{
		}

		/// <inheritdoc />
		/// <summary>
		///     Execute the stop of the scheduler.
		/// </summary>
		protected override void ExecuteStop()
		{
		}

		/// <summary>
		///     Force the scheduler to execute the pipeline.
		/// </summary>
		/// <param name="fileSource">The file source that will be processed.</param>
		/// <exception cref="SchedulerStateException">
		///     If the scheduler is not running. Call <see cref="BaseScheduler.Start" />
		/// </exception>
		public virtual void Fire(IFileSource fileSource)
		{
			if (IsRunning)
				Execute(fileSource);
			else
				throw new SchedulerStateException("The scheduler is not running.");
		}

		/// <summary>
		///     A method that ignores the parameters but is handy for adding it to an external event.
		/// </summary>
		/// <param name="sender">Ignored.</param>
		/// <param name="args">Ignored.</param>
		/// <param name="fileSource">The file source that will be processed.</param>
		public virtual void Fire(object sender, EventArgs args, IFileSource fileSource)
		{
			Fire(fileSource);
		}
	}
}