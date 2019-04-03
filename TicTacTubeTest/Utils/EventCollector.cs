using System;
using System.Collections.Generic;
using System.Threading;

namespace TicTacTubeTest.Utils
{
	public class EventCollector<T> where T : EventArgs
	{
		private readonly Predicate<T> _filter;
		private readonly List<T> _events;

		private readonly List<(Predicate<T> condition, ManualResetEventSlim wait)> _waitingClients;

		public IReadOnlyList<T> Events { get; }

		public EventCollector() : this(e => true)
		{
		}

		public EventCollector(Predicate<T> filter)
		{
			_filter = filter;

			_waitingClients = new List<(Predicate<T> condition, ManualResetEventSlim wait)>();
			_events = new List<T>();
			Events = _events.AsReadOnly();
		}

		public void Collect(object sender, T args)
		{
			if (!_filter(args)) return;
			lock (_events)
			{
				_events.Add(args);
				lock (_waitingClients)
				{
					for (int i = 0; i < _waitingClients.Count; i++)
					{
						if (_waitingClients[i].condition(args))
						{
							_waitingClients[i].wait.Set();
							_waitingClients.RemoveAt(i--);
						}
					}
				}
			}
		}

		public bool WaitFor(Predicate<T> condition, int timeout = -1)
		{
			lock (_events)
			{
				foreach (var curArg in Events)
				{
					if (condition(curArg)) return true;
				}

				return WaitForNew(condition, timeout);
			}
		}

		public bool WaitForNew(Predicate<T> condition, int timeout = -1)
		{
			var resetEvent = new ManualResetEventSlim();
			lock (_waitingClients)
			{
				_waitingClients.Add((condition, resetEvent));
			}

			return resetEvent.Wait(timeout);
		}
	}
}