using System;
using TicTacTubeCore.Processors.Definitions;

namespace TicTacTubeCore.Processors.Builders
{
	/// <summary>
	///     A data processor builder that builds <see cref="IDataProcessor" /> based on a <see cref="Func{TResult}" />.
	/// </summary>
	public class LambdaDataProcessorBuilder : BaseDataProcessorBuilder
	{
		/// <summary>
		///     The method that is used to create a data processor (see <see cref="IDataProcessor" />).
		/// </summary>
		protected readonly Func<IDataProcessor> CreationMethod;

		/// <summary>
		///     Create a new builder that uses a function to create a data processor (<see cref="IDataProcessor" />).
		/// </summary>
		/// <param name="creationMethod">The method that is used to create a data processor.</param>
		public LambdaDataProcessorBuilder(Func<IDataProcessor> creationMethod)
		{
			CreationMethod = creationMethod ?? throw new ArgumentNullException(nameof(creationMethod));
		}

		/// <inheritdoc />
		public override IDataProcessor Build()
		{
			return CreationMethod();
		}
	}
}