using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors
{
	/// <summary>
	///     A data processor that actually processes data.
	/// </summary>
	public abstract class BaseDataProcessor : IDataProcessor
	{
		/// <inheritdoc />
		public IDataProcessor Build()
		{
			return this;
		}

		/// <inheritdoc />
		public abstract IFileSource Execute(IFileSource fileSoure);

		/// <summary>
		///     Check whether the object is a builder or not. (Hint: it is never a builder).
		/// </summary>
		public bool IsBuilder => false;
	}
}