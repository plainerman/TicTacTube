using System;
using System.Collections.Generic;
using TicTacTubeCore.Sources.Files.External;

namespace TicTacTubeCore.Sources.Files.Comparer
{
	/// <summary>
	/// This comparer can compare two external file sources.
	/// The issue with external file sources is, that often they are not acquired yet (i.e. not downloaded).
	/// The downloading process often determines the actual path. So this comparer compares based on
	/// <see cref="IExternalFileSource.ExternalSource"/>. If the path has already been set, it uses another defined comparer.
	/// </summary>
	public class ExternalSourceComparer : IEqualityComparer<IFileSource>
	{
		private readonly IEqualityComparer<IFileSource> _other;

		/// <summary>
		/// Create a new comparer that use a <see cref="NameFileSourceComparer"/> if the source has already been acquired.
		/// </summary>
		public ExternalSourceComparer() : this(new NameFileSourceComparer())
		{
		}

		/// <summary>
		/// Create a new comparer that uses a defined an <paramref name="other"/> comparer, that will be used to compare
		/// if the source has already been acquired.
		/// </summary>
		/// <param name="other">A comparer that compares two file sources.</param>
		public ExternalSourceComparer(IEqualityComparer<IFileSource> other)
		{
			_other = other ?? throw new ArgumentNullException(nameof(other));
		}

		/// <inheritdoc />
		public bool Equals(IFileSource x, IFileSource y)
		{
			if (x?.FileInfo == null && y?.FileInfo == null)
			{
				return x?.ExternalSource?.ExternalSource == y?.ExternalSource?.ExternalSource;
			}

			return _other.Equals(x, y);
		}

		/// <inheritdoc />
		public int GetHashCode(IFileSource obj)
		{
			if (obj.FileInfo == null && obj.ExternalSource != null)
			{
				return obj.ExternalSource.ExternalSource == null ? 0 : obj.ExternalSource.ExternalSource.GetHashCode();
			}

			return _other.GetHashCode(obj);
		}
	}
}