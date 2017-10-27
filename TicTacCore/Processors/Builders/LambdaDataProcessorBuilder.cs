using System;

namespace TicTacCore.Processors.Builders
{
	/// <summary>
	///     A data processor builder that builds <see cref="IDataProcessor" /> based on a <see cref="Func{TResult}" />.
	/// </summary>
	public class LambdaDataProcessorBuilder : BaseDataProcessorBuilder
	{
		protected readonly Func<IDataProcessor> CreationMethod;

		public LambdaDataProcessorBuilder(Func<IDataProcessor> creationMethod)
		{
			CreationMethod = creationMethod ?? throw new ArgumentNullException(nameof(creationMethod));
		}

		public override IDataProcessor Build()
		{
			return CreationMethod();
		}
	}
}