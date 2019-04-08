using System;
using System.IO;
using TicTacTubeCore.Executors;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Schedulers
{
	/// <inheritdoc cref="BaseScheduler" />
	/// <summary>
	///     A scheduler that can watch a directory and trigger when something has changed.
	///     It has to be disposed.
	/// </summary>
	public class FileSystemScheduler : BaseScheduler, IDisposable
	{
		/// <summary>
		///     The path that will be watched.
		/// </summary>
		public string Path { get; }

		/// <summary>
		///     The watcher that keeps track of the tracking.
		/// </summary>
		public FileSystemWatcher Watcher { get; }

		/// <summary>
		/// Whether the scheduler should wait for the file to be readable.
		/// Is highly recommended, since the event is triggered once the file is created but if it is (e.g.) still copying,
		/// it will create issues when opening the files.
		/// </summary>
		public bool WaitForRead { get; set; } = true;

		/// <summary>
		/// Whether the scheduler should wait for the file to be writable.
		/// Use this option with caution: if the user does not have enough permission it will prevent stopping the scheduler.
		/// </summary>
		public bool WaitForWrite { get; set; } = false;

		/// <inheritdoc />
		/// <summary>
		///     Create a new scheduler that watches a given <paramref name="path" />, filters files (<paramref name="filter" /> see
		///     <see cref="P:System.IO.FileSystemWatcher.Filter" />),
		///     and the <paramref name="filters" /> that specify when to trigger (see <see cref="P:System.IO.FileSystemWatcher.NotifyFilter" />
		///     ).
		/// </summary>
		/// <param name="executor">The executor that will be used when executing the pipeline.</param>
		/// <param name="path">The path that will be watched.</param>
		/// <param name="filters">The filters that decide when to trigger.</param>
		/// <param name="filter">A filter that is applied to file names.</param>
		public FileSystemScheduler(IExecutor executor, string path,
			NotifyFilters filters = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
			                        NotifyFilters.DirectoryName, string filter = "*.*") : base(executor)
		{
			Path = path;
			Watcher = new FileSystemWatcher(path, filter) { NotifyFilter = filters };
			Watcher.Created += OnChanged;
			Watcher.Changed += OnChanged;
			Watcher.Renamed += OnChanged;
		}

		/// <inheritdoc />
		/// <summary>
		///     Create a new scheduler that watches a given <paramref name="path" />, filters files (<paramref name="filter" /> see
		///     <see cref="P:System.IO.FileSystemWatcher.Filter" />),
		///     and the <paramref name="filters" /> that specify when to trigger (see <see cref="P:System.IO.FileSystemWatcher.NotifyFilter" />
		///     ). The default executor specified in <see cref="BaseScheduler"/> will be used.
		/// </summary>
		/// <param name="path">The path that will be watched.</param>
		/// <param name="filters">The filters that decide when to trigger.</param>
		/// <param name="filter">A filter that is applied to file names.</param>
		public FileSystemScheduler(string path,
			NotifyFilters filters = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
			                        NotifyFilters.DirectoryName, string filter = "*.*") :
			this(null, path, filters, filter)
		{
		}


		/// <inheritdoc />
		protected override void ExecuteStart()
		{
			Watcher.EnableRaisingEvents = true;
		}

		/// <summary>
		///     The method that will be executed once a file changes.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The args for the triggered event.</param>
		protected virtual void OnChanged(object source, FileSystemEventArgs e)
		{
			Execute(new FileSource(e.FullPath), f =>
			{
				if (!WaitForRead && !WaitForWrite) return true;
				try
				{
					using (new FileStream(f.FileInfo.FullName, WaitForWrite ? FileMode.Append : FileMode.Open))
					{
						return true;
					}
				}
				catch
				{
					return false;
				}
			});
		}

		/// <inheritdoc />
		protected override void ExecuteStop()
		{
			Watcher.EnableRaisingEvents = false;
		}

		/// <summary>
		///     Dispose this scheduler.
		/// </summary>
		/// <param name="disposing">The boolean whether to dispose.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Watcher.Changed -= OnChanged;
				Watcher.Created -= OnChanged;
				Watcher.Renamed -= OnChanged;
				Watcher.Dispose();
			}
		}

		/// <inheritdoc />
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}