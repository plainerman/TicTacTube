namespace TicTacTubeCore.Utils
{
	/// <summary>
	///     A convenient utils class that provides useful functions for array usage.
	/// </summary>
	public static class ArrayUtils
	{
		/// <summary>
		///     Multiplex a single object into a complete array of this object.
		/// </summary>
		/// <typeparam name="T">The type of the array</typeparam>
		/// <param name="obj">The object that willbe converted to an array.</param>
		/// <param name="count">The length of the array.</param>
		/// <returns>
		///     The newly created array with the length of <paramref name="count" />, where every element is
		///     <paramref name="obj" />.
		/// </returns>
		public static T[] Multiplex<T>(T obj, int count)
		{
			var ret = new T[count];
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = obj;
			}
			return ret;
		}
	}
}