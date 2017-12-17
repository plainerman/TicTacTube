namespace TicTacTubeCore.Processors.Media
{
	/// <summary>
	/// An interface for a class that can maniupulate a string.
	/// </summary>
	public interface IStringProcessor
	{
		/// <summary>
		/// Process a given string and return a new one. 
		/// </summary>
		/// <param name="str">The string that will be processed.</param>
		/// <returns>A string that has been processed (i.e. manipulated).</returns>
		string Process(string str);
	}
}