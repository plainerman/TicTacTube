﻿using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Definitions
{
	/// <summary>
	///     A data processor that actually processes data.
	/// </summary>
	public abstract class BaseDataProcessor : IDataProcessor
	{
		/// <inheritdoc />
		public IDataProcessor Build() => this;

		/// <inheritdoc />
		public abstract IFileSource Execute(IFileSource fileSource);

		/// <summary>
		///     Check whether the object is a builder or not. (Hint: it is never a builder).
		/// </summary>
		public bool IsBuilder => false;
	}
}