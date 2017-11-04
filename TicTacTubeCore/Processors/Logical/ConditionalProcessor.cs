using System;
using TicTacTubeCore.Processors.Definitions;
using TicTacTubeCore.Sources.Files;

namespace TicTacTubeCore.Processors.Logical
{
	/// <summary>
	/// A dataprocessor that can decide between two data processors to execute from.
	/// </summary>
	public class ConditionalProcessor : BaseDataProcessor
	{
		private readonly Func<IFileSource, bool> _condition;
		private readonly IDataProcessorOrBuilder _dataProcessorA;
		private readonly IDataProcessorOrBuilder _dataProcessorB;

		/// <summary>
		/// Create a new conditional processor with a given condition.
		/// 
		/// If the condition is <c>true</c>, <paramref name="dataProcessorA"/> will be used.
		/// If the condition is <c>false</c>, <paramref name="dataProcessorB"/> will be used.
		/// 
		/// One of the two data processors may be <c>null</c> (but not both). If a given data processor is null, it will be not be executed.
		/// 
		/// </summary>
		/// <param name="condition">The condition that is used to evaluate. May not be <c>null</c>.</param>
		/// <param name="dataProcessorA">The data processor that is executed when the condition evaluates to <c>true</c>.</param>
		/// <param name="dataProcessorB">The data processor that is executed when the condition evaluates to <c>false</c>.</param>
		public ConditionalProcessor(Func<IFileSource, bool> condition, IDataProcessorOrBuilder dataProcessorA,
			IDataProcessorOrBuilder dataProcessorB)
		{
			if (dataProcessorA == null && dataProcessorB == null)
			{
				throw new ArgumentNullException(nameof(dataProcessorA), "Both parameters may not be null.");
			}

			_condition = condition ?? throw new ArgumentNullException(nameof(condition));
			_dataProcessorA = dataProcessorA;
			_dataProcessorB = dataProcessorB;
		}

		/// <summary>
		/// Create a new conditional processor with a given condition.
		/// 
		/// If the condition is <c>true</c>, <paramref name="dataProcessorA"/> will be used and executed - otherwise nothing will be executed.
		/// 		/// 
		/// </summary>
		/// <param name="condition">The condition that is used to evaluate. May not be <c>null</c>.</param>
		/// <param name="dataProcessorA">The data processor that is executed when the condition evaluates to <c>true</c>. May not be <c>null</c>.</param>
		public ConditionalProcessor(Func<IFileSource, bool> condition, IDataProcessorOrBuilder dataProcessorA) : this(
			condition, dataProcessorA, null)
		{

		}

		/// <inheritdoc />
		public override IFileSource Execute(IFileSource fileSoure)
		{
			var eval = _condition(fileSoure);
			if (eval)
			{
				if (_dataProcessorA != null)
				{
					return _dataProcessorA.Build().Execute(fileSoure);
				}
			}
			else
			{
				if (_dataProcessorB != null)
				{
					return _dataProcessorB.Build().Execute(fileSoure);
				}
			}

			return fileSoure;
		}
	}
}