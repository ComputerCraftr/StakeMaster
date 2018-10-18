﻿// ******************************* Module Header *******************************
// Module Name: TransactionHelper.cs
// Project:     StakeMasterBusinessLogic
// Copyright (c) Michael Goldfinger.
// 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// *****************************************************************************

namespace StakeMaster.BusinessLogic
{
	using JetBrains.Annotations;

	/// <summary>
	///     Contains methods for handling transaction sizes.
	/// </summary>
	[PublicAPI]
	public sealed class TransactionHelper
	{
		/// <inheritdoc />
		public TransactionHelper(int inputSize, int outputSize, int transactionOverhead, int freeTransactionByteLimit)
		{
			InputSize = inputSize;
			OutputSize = outputSize;
			TransactionOverhead = transactionOverhead;
			FreeTransactionByteLimit = freeTransactionByteLimit;
		}

		/// <summary>
		///     Gets the allowed size for zero fee transactions.
		/// </summary>
		public int FreeTransactionByteLimit { get; }

		/// <summary>
		///     Gets the size in bytes for one input.
		/// </summary>
		public int InputSize { get; }

		/// <summary>
		///     Gets the size in bytes for one output.
		/// </summary>
		public int OutputSize { get; }

		/// <summary>
		///     Gets the size in bytes for transaction overhead.
		/// </summary>
		public int TransactionOverhead { get; }

		/// <summary>
		///     Gets the amount of inputs that can be included in a transaction.
		/// </summary>
		/// <param name="numberOfOutputs">
		///     The number of outputs already used in the transaction.
		/// </param>
		/// <returns>
		///     The amount of possible inputs.
		/// </returns>
		public int GetMaxPossibleInputCountForFreeTransaction(int numberOfOutputs)
		{
			int size = GetTransactionSize(1, numberOfOutputs);
			if (FreeTransactionByteLimit < size)
			{
				throw new FreeTransactionNotPossibleException(1, numberOfOutputs, size, FreeTransactionByteLimit);
			}

			var count = 1;
			while (GetTransactionSize(count + 1, numberOfOutputs) <= FreeTransactionByteLimit)
			{
				++count;
			}

			return count;
		}

		/// <summary>
		///     Gets the amount of outputs that can be included in a transaction.
		/// </summary>
		/// <param name="numberOfInputs">
		///     The number of inputs already used in the transaction.
		/// </param>
		/// <returns>
		///     The amount of possible outputs.
		/// </returns>
		public int GetMaxPossibleOutputCountForFreeTransaction(int numberOfInputs)
		{
			int size = GetTransactionSize(numberOfInputs, 1);
			if (FreeTransactionByteLimit < size)
			{
				throw new FreeTransactionNotPossibleException(numberOfInputs, 1, size, FreeTransactionByteLimit);
			}

			var count = 1;
			while (GetTransactionSize(numberOfInputs, count + 1) <= FreeTransactionByteLimit)
			{
				++count;
			}

			return count;
		}

		/// <summary>
		///     Gets the size of a transaction.
		/// </summary>
		/// <param name="numberOfInputs">
		///     The number of inputs used in the transaction.
		/// </param>
		/// <param name="numberOfOutputs">
		///     The number of outputs used in the transaction
		/// </param>
		/// <returns>
		///     The calculated transaction size;
		/// </returns>
		public int GetTransactionSize(int numberOfInputs, int numberOfOutputs) => (numberOfInputs * InputSize) + (numberOfOutputs * OutputSize) + TransactionOverhead;
	}
}