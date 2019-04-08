using System;
using System.Collections.Generic;
using System.Linq;
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

		public bool WaitFor(Predicate<T> condition, int timeout = -1, bool discardExisting = false)
		{
			var resetEvent = new ManualResetEventSlim();

			lock (_events)
			{
				if (!discardExisting && Events.Any(curArg => condition(curArg)))
				{
					return true;
				}

				lock (_waitingClients)
				{
					_waitingClients.Add((condition, resetEvent));
				}
			}

			return resetEvent.Wait(timeout);
		}
	}
}