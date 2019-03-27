using System.Collections.Generic;

namespace TicTacTubeCore.Sources.Files.Comparer
{
	/// <inheritdoc />
	/// <summary>
	/// This comparer compares the full file name of two file sources to check for equality.
	/// </summary>
	public class NameFileSourceComparer : IEqualityComparer<IFileSource>
	{
		/// <inheritdoc />
		/// <summary>
		///	Check if <see ref="IFileSource.FileInfo.FullName"/> is equal. 
		/// </summary>
		public bool Equals(IFileSource x, IFileSource y)
		{
			if (x == null && y == null) return true;
			if (x == null || y == null) return false;

			//TODO: ensure that it works properly with lazy external sources

			return x.FileInfo?.FullName == y.FileInfo?.FullName;
		}

		/// <inheritdoc />
		public int GetHashCode(IFileSource obj) => obj.FullFileName.GetHashCode();
	}
}