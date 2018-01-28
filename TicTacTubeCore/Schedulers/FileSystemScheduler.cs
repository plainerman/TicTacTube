using System;
using System.IO;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Schedulers
{
	/// <summary>
	/// A scheduler that can watch a directory and trigger when something has changed. 
	/// It has to be disposed.
	/// </summary>
	public class FileSystemScheduler : BaseScheduler, IDisposable
	{
		/// <summary>
		/// The path that will be watched.
		/// </summary>
		public string Path { get; }

		/// <summary>
		/// The watcher that keeps track of the tracking.
		/// </summary>
		public FileSystemWatcher Watcher { get; }

		/// <summary>
		/// Create a new scheduler that watches a given <paramref name="path"/>, filters files (<paramref name="filter"/> see <see cref="FileSystemWatcher.Filter"/>),
		/// and the <paramref name="filters"/> that specify when to trigger (see <see cref="FileSystemWatcher.NotifyFilter"/>).
		/// </summary>
		/// <param name="path">The path that will be watched.</param>
		/// <param name="filters">The filters that decide when to trigger.</param>
		/// <param name="filter">A filter that is applied to file names.</param>
		public FileSystemScheduler(string path, NotifyFilters filters = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName, string filter = "*.*")
		{
			Path = path;
			Watcher = new FileSystemWatcher(path, filter) { NotifyFilter = filters };
			Watcher.Changed += OnChanged;
			Watcher.Created += OnChanged;
			Watcher.Renamed += OnChanged;
		}

		/// <inheritdoc />
		protected override void ExecuteStart()
		{
			Watcher.EnableRaisingEvents = true;
		}

		/// <summary>
		/// The method that will be excecuted once a file changes.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The args for the triggered event.</param>
		protected virtual void OnChanged(object source, FileSystemEventArgs e)
		{
			Execute(new FileSource(e.FullPath));
		}

		/// <inheritdoc />
		protected override void ExecuteStop()
		{
			Watcher.EnableRaisingEvents = false;
		}

		/// <summary>
		/// Dispose this scheduler.
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