using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Basics.Models
{
    /// <summary>
    ///     Base search criteria class that supports only pagination.
    /// </summary>
    [DebuggerDisplay("Starting at {StartRecord ?? 0}, max {RecordCount ?? int.MaxValue} records")]
    public abstract class SearchCriteria
    {
        /// <summary>
        ///     Zero-based index of the starting record to retrieve. Defaults to 0 if not specified.
        /// </summary>
        public int? StartRecord { get; set; }

        /// <summary>
        ///     Number of records to retrieve. Retrieves all records if not specified.
        /// </summary>
        public int? RecordCount { get; set; }

        /// <summary>
        ///     Returns whether the search criteria specifies pagination criteria.
        /// </summary>
        /// <remarks>
        ///     This member is a method and not a property, to avoid unnecessarily serializing it. While we can use
        ///     specialized attributes to avoid serialization, those are framework-specific, and by making it a method, we get all
        ///     the same benefits without having to select a specific serialization framework to handle.
        /// </remarks>
        public bool RequiresPagination()
        {
            bool requiresPagination = StartRecord.HasValue && StartRecord.Value > 0;
            if (requiresPagination)
                return true;
            return RecordCount.HasValue && RecordCount.Value > 0;
        }
    }

    /// <summary>
    ///     Base search criteria class that supports sorting, in addition to pagination.
    /// </summary>
    /// <typeparam name="TSortField">Enum representing the fields that can be sorted.</typeparam>
    public abstract class SearchCriteria<TSortField> : SearchCriteria
        where TSortField : struct, IComparable
    {
        public SortSpecs<TSortField> SortSpecs { get; } = new SortSpecs<TSortField>();

        public SearchCriteria<TSortField> SortBy(TSortField sortField, SortOrder sortOrder = SortOrder.Ascending)
        {
            SortSpecs.Add(sortField, sortOrder);
            return this;
        }
    }

    public abstract class SearchCriteria<TFilterField, TSortField> : SearchCriteria<TSortField>
        where TFilterField : struct, IComparable
        where TSortField : struct, IComparable
    {
        public FilterCriterion<TFilterField> Filters { get; } = new FilterCriterion<TFilterField>();

        /// <summary>
        ///     Returns whether the search criteria specifies filtering criteria.
        /// </summary>
        public bool RequiresFiltering() => Filters.Count > 0;
    }

    public static class SearchCriteriaExtensions
    {
        public static Tuple<SearchCriteria<TFilterField, TSortField>, TFilterField> Where<TFilterField, TSortField>(
            this SearchCriteria<TFilterField, TSortField> criteria, TFilterField field)
            where TFilterField : struct, IComparable
            where TSortField : struct, IComparable
        {
            return Tuple.Create(criteria, field);
        }

        public static SearchCriteria<TFilterField, TSortField> IsEqualTo<TFilterField, TSortField, T>(
            this Tuple<SearchCriteria<TFilterField, TSortField>, TFilterField> tuple, T value)
            where TFilterField : struct, IComparable
            where TSortField : struct, IComparable
        {
            tuple.Item1.Filters.Add(tuple.Item2, FilterOperation.Equals, value);
            return tuple.Item1;
        }

        public static SearchCriteria<TFilterField, TSortField> IsNotEqualTo<TFilterField, TSortField, T>(
            this Tuple<SearchCriteria<TFilterField, TSortField>, TFilterField> tuple, T value)
            where TFilterField : struct, IComparable
            where TSortField : struct, IComparable
        {
            tuple.Item1.Filters.Add(tuple.Item2, FilterOperation.DoesNotEqual, value);
            return tuple.Item1;
        }

        public static SearchCriteria<TFilterField, TSortField> IsGreaterThan<TFilterField, TSortField, T>(
            this Tuple<SearchCriteria<TFilterField, TSortField>, TFilterField> tuple, T value)
            where TFilterField : struct, IComparable
            where TSortField : struct, IComparable
        {
            tuple.Item1.Filters.Add(tuple.Item2, FilterOperation.GreaterThan, value);
            return tuple.Item1;
        }

        public static SearchCriteria<TFilterField, TSortField> IsGreaterThanOrEqualTo<TFilterField, TSortField, T>(
            this Tuple<SearchCriteria<TFilterField, TSortField>, TFilterField> tuple, T value)
            where TFilterField : struct, IComparable
            where TSortField : struct, IComparable
        {
            tuple.Item1.Filters.Add(tuple.Item2, FilterOperation.GreaterThanOrEqual, value);
            return tuple.Item1;
        }

        public static SearchCriteria<TFilterField, TSortField> IsLessThan<TFilterField, TSortField, T>(
            this Tuple<SearchCriteria<TFilterField, TSortField>, TFilterField> tuple, T value)
            where TFilterField : struct, IComparable
            where TSortField : struct, IComparable
        {
            tuple.Item1.Filters.Add(tuple.Item2, FilterOperation.LessThan, value);
            return tuple.Item1;
        }

        public static SearchCriteria<TFilterField, TSortField> IsLessThanOrEqualTo<TFilterField, TSortField, T>(
            this Tuple<SearchCriteria<TFilterField, TSortField>, TFilterField> tuple, T value)
            where TFilterField : struct, IComparable
            where TSortField : struct, IComparable
        {
            tuple.Item1.Filters.Add(tuple.Item2, FilterOperation.LessThanOrEqual, value);
            return tuple.Item1;
        }

        public static SearchCriteria<TFilterField, TSortField> IsLike<TFilterField, TSortField>(
            this Tuple<SearchCriteria<TFilterField, TSortField>, TFilterField> tuple, string value)
            where TFilterField : struct, IComparable
            where TSortField : struct, IComparable
        {
            tuple.Item1.Filters.Add(tuple.Item2, FilterOperation.Like, value);
            return tuple.Item1;
        }
    }

    [DebuggerDisplay("{Field} {Operation} {Value}")]
    public sealed class FilterCriteria<TFilterField>
        where TFilterField : struct, IComparable
    {
        public FilterCriteria(TFilterField field, FilterOperation operation, object value)
        {
            Field = field;
            Operation = operation;
            Value = value;
        }

        public TFilterField Field { get; }
        public FilterOperation Operation { get; }
        public object Value { get; }
    }

    public enum FilterOperation
    {
        Equals,
        DoesNotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Like
    }

    public sealed class FilterCriterion<TFilterField> : Collection<FilterCriteria<TFilterField>>
        where TFilterField : struct, IComparable
    {
        public void Add<T>(TFilterField field, FilterOperation operation, T value)
        {
            Add(new FilterCriteria<TFilterField>(field, operation, value));
        }
    }
}
