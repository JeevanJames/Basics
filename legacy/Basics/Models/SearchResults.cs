using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Basics.Models
{
    [DebuggerDisplay("{Data.Count} of {TotalCount}")]
    public abstract class SearchResults<T>
    {
        protected SearchResults(IReadOnlyList<T> data, int totalCount)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            Data = data;
            TotalCount = totalCount;
        }

        protected SearchResults(IEnumerable<T> data, int totalCount)
            : this(new List<T>(data ?? Enumerable.Empty<T>()), totalCount)
        {
        }

        /// <summary>
        ///     Data records returned from the search query.
        /// </summary>
        public IReadOnlyList<T> Data { get; }

        /// <summary>
        ///     Total number of records returned from the search query. This is normally the same as the number of items in the
        ///     Data property, unless pagination criteria is applied.
        /// </summary>
        public int TotalCount { get; }

        //TODO: Add properties for pagination criteria
    }
}