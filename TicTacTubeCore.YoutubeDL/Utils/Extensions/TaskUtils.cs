using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TicTacTubeCore.YoutubeDL.Utils.Extensions
{
	/// <summary>
	/// An utils class for task creation / monitoring.
	/// </summary>
	public class TaskUtils
	{
		/// <summary>
		/// Let a process run inside a task asynchronously.
		/// </summary>
		/// <param name="fileName">The process that will be executed.</param>
		/// <returns>The task.</returns>
		public static Task RunProcessAsync(string fileName)
		{
			// there is no non-generic TaskCompletionSource
			var tcs = new TaskCompletionSource<bool>();

			var process = new Process
			{
				StartInfo = { FileName = fileName },
				EnableRaisingEvents = true
			};

			process.Exited += (sender, args) =>
			{
				tcs.SetResult(true);
				process.Dispose();
			};

			process.Start();

			return tcs.Task;
		}

		/// <summary>
		/// Let a process run inside a task asynchronously.
		/// </summary>
		/// <param name="process">The process that will be wrapped.</param>
		/// <returns>The task.</returns>
		public static Task RunProcessInTask(Process process)
		{
			var tcs = new TaskCompletionSource<bool>();


			process.Exited += OnProcesOnExited;

			return tcs.Task;

			void OnProcesOnExited(object sender, EventArgs args)
			{
				process.Exited -= OnProcesOnExited;
				tcs.SetResult(true);
				process.Dispose();
				process = null;
			}
		}
	}
}