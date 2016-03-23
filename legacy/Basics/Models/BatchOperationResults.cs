using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Basics.Models
{
    /// <summary>
    ///     Represents the result of a batch operation where some items in the batch could be successful and others could fail.
    /// </summary>
    /// <typeparam name="TSuccessModel">
    ///     The model to return for successful operations. Typically the same model passed as input
    ///     to the batch operation.
    /// </typeparam>
    /// <typeparam name="TFailureModel">The model to return for failed operations.</typeparam>
    [DebuggerDisplay("{Successes.Count} successes; {Failures.Count} failures")]
    public abstract class BatchOperationResults<TSuccessModel, TFailureModel>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<TSuccessModel> _successes;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<BatchOperationError<TFailureModel>> _failures;

        protected BatchOperationResults(IEnumerable<TSuccessModel> successes,
            IEnumerable<BatchOperationError<TFailureModel>> failures)
        {
            _successes = new List<TSuccessModel>(successes ?? Enumerable.Empty<TSuccessModel>());
            _failures = new List<BatchOperationError<TFailureModel>>(failures ??
                Enumerable.Empty<BatchOperationError<TFailureModel>>());
        }

        /// <summary>
        ///     Gets the collection of models whose operations were successful.
        /// </summary>
        public IReadOnlyList<TSuccessModel> Successes => _successes;

        /// <summary>
        ///     Gets the collection of models whose operations failed. Each item is wrapped in a BatchOperationError object, which
        ///     also specifies the error object.
        /// </summary>
        public IReadOnlyList<BatchOperationError<TFailureModel>> Failures => _failures;

        /// <summary>
        ///     Returns whether all operations were successful. This property returns true even if there were no operations.
        /// </summary>
        public bool IsSuccessful() => _failures.Count == 0;

        /// <summary>
        ///     Returns whether all operations failed.
        /// </summary>
        public bool HasFailed() => _successes.Count == 0 && _failures.Count > 0;

        /// <summary>
        ///     Returns whether some operations succeeded and some failed.
        /// </summary>
        public bool HasMixedResults() => _successes.Count > 0 && _failures.Count > 0;
    }

    /// <summary>
    ///     Represents a batch operation result where both the success and failure models are of the same type.
    /// </summary>
    /// <typeparam name="T">The model type representing success and failure.</typeparam>
    public abstract class BatchOperationResults<T> : BatchOperationResults<T, T>
    {
        protected BatchOperationResults(IEnumerable<T> successes, IEnumerable<BatchOperationError<T>> failures)
            : base(successes, failures)
        {
        }
    }

    public sealed class BatchOperationError<T>
    {
        public BatchOperationError(T item, object error)
        {
            Item = item;
            Error = error;
        }

        public T Item { get; }
        public object Error { get; }
    }

    public static class BatchOperationExtensions
    {
        /// <summary>
        ///     Runs a batch operation over a collection of items, captures any errors and returns a
        ///     <see cref="BatchOperationResults{TSuccessModel,TFailureModel}" /> result.
        /// </summary>
        /// <typeparam name="TModel">Data type of the items to iterate through.</typeparam>
        /// <typeparam name="TResults">Type of the <see cref="BatchOperationResults{TSuccessModel,TFailureModel}" /> object.</typeparam>
        /// <param name="source">Collection of items to iterate through.</param>
        /// <param name="action">Delegate that defines the operation to run on each item.</param>
        /// <returns>
        ///     A <see cref="BatchOperationResults{TSuccessModel,TFailureModel}" /> instance that contains the results from
        ///     batch run.
        /// </returns>
        public static TResults RunBatch<TModel, TResults>(this IEnumerable<TModel> source, Func<TModel, bool> action)
            where TResults : BatchOperationResults<TModel>
        {
            var successes = new List<TModel>();
            var failures = new List<BatchOperationError<TModel>>();

            foreach (TModel model in source)
            {
                try
                {
                    if (action(model))
                        successes.Add(model);
                    else
                        failures.Add(new BatchOperationError<TModel>(model, $"Unknown error on model {model}."));
                } catch (Exception ex)
                {
                    failures.Add(new BatchOperationError<TModel>(model, ex));
                }
            }

            return (TResults)Activator.CreateInstance(typeof(TResults), successes, failures);
        }

        /// <summary>
        ///     Runs a batch operation over a collection of items, captures any errors and returns a
        ///     <see cref="BatchOperationResults{TSuccessModel,TFailureModel}" /> result.
        /// </summary>
        /// <typeparam name="TModel">Data type of the items to iterate through.</typeparam>
        /// <typeparam name="TResults">Type of the <see cref="BatchOperationResults{TSuccessModel,TFailureModel}" /> object.</typeparam>
        /// <param name="source">Collection of items to iterate through.</param>
        /// <param name="action">Delegate that defines the operation to run on each item.</param>
        /// <returns>
        ///     A <see cref="BatchOperationResults{TSuccessModel,TFailureModel}" /> instance that contains the results from
        ///     batch run.
        /// </returns>
        public static async Task<TResults> RunBatch<TModel, TResults>(this IEnumerable<TModel> source,
            Func<TModel, Task<bool>> action)
            where TResults : BatchOperationResults<TModel>
        {
            var successes = new List<TModel>();
            var failures = new List<BatchOperationError<TModel>>();

            foreach (TModel model in source)
            {
                try
                {
                    if (await action(model).ConfigureAwait(false))
                        successes.Add(model);
                    else
                        failures.Add(new BatchOperationError<TModel>(model, $"Unknown error on model {model}."));
                } catch (Exception ex)
                {
                    failures.Add(new BatchOperationError<TModel>(model, ex));
                }
            }

            return (TResults)Activator.CreateInstance(typeof(TResults), successes, failures);
        }
    }
}
