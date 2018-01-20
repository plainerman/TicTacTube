namespace TicTacTubeCore.Processors.Media
{
	/// <summary>
	/// An interface that defines methods on how to combine multiple media info.
	/// </summary>
	/// <typeparam name="T">The type of media that will be merged.</typeparam>
	public interface IMediaInfoMerger<T> where T : IMediaInfo
	{
		/// <summary>
		/// Merge two <see cref="IMediaInfo"/>. Depending on the implementation it may decide which info to trust more.
		/// Generally speaking, the first info is the more trustworthy song (except specified otherwise by the implementation).
		/// It can also be decided whether the data should be fetched greedy.
		/// </summary>
		/// <param name="info1">The primary media info (i.e. higher priority). The other info will be added / overrides this info. May not be <c>null</c>.</param>
		/// <param name="info2">The media info with less priority. May not be <c>null</c>.</param>
		/// <param name="greedy">If <paramref name="greedy"/> is <c>true</c>, data that is <c>null</c> will be added to the new info, regardless of whether the other source is trustworthy or not.
		/// In other words, if <paramref name="greedy"/> is <c>true</c>, merge will return more data, but it may be completely wrong.
		/// </param>
		/// <returns>A merged media info containing a best guess which data is correct.</returns>
		T Merge(T info1, T info2, bool greedy);

		/// <summary>
		/// Merge two <see cref="IMediaInfo"/>. Depending on the implementation it may decide which info to trust more.
		/// Generally speaking, the first info is the more trustworthy song (except specified otherwise by the implementation) — all others are of descending priority.
		/// It can also be decided whether the data should be fetched greedy.
		/// </summary>
		/// <param name="info1">The primary media info (i.e. higher priority). The other info will be added / overrides this info.</param>
		/// <param name="info2">The media info with less priority.</param>
		/// <param name="info3">The media info with less priority.</param>
		/// <param name="greedy">If <paramref name="greedy"/> is <c>true</c>, data that is <c>null</c> will be added to the new info, regardless of whether the other source is trustworthy or not.
		/// In other words, if <paramref name="greedy"/> is <c>true</c>, merge will return more data, but it may be completely wrong.
		/// </param>
		/// <param name="infos">An arbitry amount of additional infos in descending order.</param>
		/// <returns>A merged media info containing a best guess which data is correct.</returns>
		T Merge(T info1, T info2, T info3, bool greedy, params T[] infos);
	}
}