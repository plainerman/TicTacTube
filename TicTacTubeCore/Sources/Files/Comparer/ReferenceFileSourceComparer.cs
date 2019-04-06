using System.Collections.Generic;

namespace TicTacTubeCore.Sources.Files.Comparer
{
	/// <inheritdoc />
	/// <summary>
	/// This comparer uses the reference of two file sources to determine whether they are equal or not.
	/// </summary>
	public class ReferenceFileSourceComparer : IEqualityComparer<IFileSource>
	{
		/// <inheritdoc />
		/// <summary>Check the reference of the two file sources and determine whether they are equal.</summary>
		public bool Equals(IFileSource x, IFileSource y) => ReferenceEquals(x, y);

		/// <inheritdoc />
		public int GetHashCode(IFileSource obj) => obj.GetHashCode();
	}
}