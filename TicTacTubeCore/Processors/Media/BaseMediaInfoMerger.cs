using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacTubeCore.Processors.Media
{
	/// <summary>
	///     The base implementation for a <see cref="IMediaInfoMerger{T}" />.
	/// </summary>
	/// <typeparam name="T">The type of media that will be merged.</typeparam>
	public abstract class BaseMediaInfoMerger<T> : IMediaInfoMerger<T> where T : IMediaInfo
	{
		/// <summary>
		///     Merge multiple infos with the same <paramref name="greedy" /> parameter. Basically all infos are aggregated and
		///     pair-wise merged.
		/// </summary>
		/// <param name="infos">The infos that will be merged.</param>
		/// <param name="greedy">
		///     If <paramref name="greedy" /> is <c>true</c>, data that is <c>null</c> will be added to the new info, regardless of
		///     whether the other source is trustworthy or not.
		///     In other words, if <paramref name="greedy" /> is <c>true</c>, merge will return more data, but it may be completely
		///     wrong.
		/// </param>
		/// <returns>A merged media info containing a best guess which data is correct.</returns>
		protected virtual T MergeUnchecked(T[] infos, bool greedy)
		{
			return infos.Aggregate((info1, info2) => Merge(info1, info2, greedy));
		}

		/// <summary>
		///     Merge two data objects depending on whether it is a trusty source (<paramref name="trusted" />) and if it should be
		///     <paramref name="greedy" />.
		///     Either object a or object b will be returned.
		///     If it is a trusted object, object <paramref name="b" /> is prefered — otherwise object <paramref name="a" />.
		/// </summary>
		/// <typeparam name="TOther">The type of the object that will be merged.</typeparam>
		/// <param name="a">The default object that will be used.</param>
		/// <param name="b">
		///     The object that may override <paramref name="a" />, if <paramref name="a" /> is <c>null</c> (and
		///     <paramref name="greedy" /> is <c>true</c>) or it is a <paramref name="trusted" /> source.
		/// </param>
		/// <param name="trusted">
		///     If the source is trusted, <paramref name="b" /> will override <paramref name="a" /> (as long as
		///     <paramref name="a" /> is <c>null</c>).
		/// </param>
		/// <param name="greedy">
		///     If <paramref name="greedy" /> is <c>true</c>, data that is <c>null</c> will be added to the new info, regardless of
		///     whether the other source is trustworthy or not.
		///     In other words, if <paramref name="greedy" /> is <c>true</c>, merge will return more data, but it may be completely
		///     wrong.
		/// </param>
		/// <returns>A merged media info containing a best guess which data is correct.</returns>
		protected virtual TOther MergeData<TOther>(TOther a, TOther b, bool trusted, bool greedy)
		{
			if (a == null && greedy) return b;
			if (b != null && trusted) return b;
			return a;
		}

		/// <summary>
		///     Merge two arrays depending on whether it is a trusty source (<paramref name="trusted" />) and if it should be
		///     <paramref name="greedy" />.
		///     Array <paramref name="a" /> or <paramref name="b" /> will be returned — or they may be merged.
		///     If both arrays are populated and the source is <paramref name="trusted" /> they will be merged. Otherwise it will
		///     be handled as in <see cref="MergeData{TOther}" />, only that an empty array is handled like a <c>null</c> array.
		///     Arrays can also be merged with <see cref="MergeData{TOther}" />, if it should be all or nothing.
		/// </summary>
		/// <typeparam name="TOther">The type of the array that will be merged.</typeparam>
		/// <param name="a">The default object that will be used.</param>
		/// <param name="b">The object <paramref name="a" /> will be expanded with (or replaces it, or is ignored).</param>
		/// <param name="trusted">If the source is trusted, <paramref name="b" /> will expand <paramref name="a" />.</param>
		/// <param name="greedy">
		///     If <paramref name="greedy" /> is <c>true</c>, data that is <c>null</c> will be added to the new info, regardless of
		///     whether the other source is trustworthy or not.
		///     In other words, if <paramref name="greedy" /> is <c>true</c>, merge will return more data, but it may be completely
		///     wrong.
		/// </param>
		/// <returns>A merged media info containing a best guess which data is correct.</returns>
		protected virtual TOther[] MergeDataArray<TOther>(TOther[] a, TOther[] b, bool trusted, bool greedy)
		{
			if ((a == null || a.Length <= 0) && greedy) return b;
			if (b == null || b.Length <= 0 || !trusted) return a;
			if (a == null) return b;

			var bAsList = b.ToList();
			bAsList.AddRange(a);

			return bAsList.Distinct().ToArray();
		}

		/// <inheritdoc />
		public abstract T Merge(T info1, T info2, bool greedy);

		/// <inheritdoc />
		public T Merge(T info1, T info2, T info3, bool greedy, params T[] infos)
		{
			var allInfos = new List<T> { info1, info2, info3 };
			allInfos.AddRange(infos);

			infos = allInfos.Where(i => i != null).ToArray();

			if (infos.Length < 2)
				throw new ArgumentException("Too few infos specified. An info that is null will be ignored.");

			return MergeUnchecked(infos, greedy);
		}
	}
}