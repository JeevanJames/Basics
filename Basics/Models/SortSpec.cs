using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Basics.Models
{
    /// <summary>
    ///     Represents a field sorting specification, which includes the field to sort by and the sort direction.
    /// </summary>
    /// <typeparam name="TSortField">The enum that specifies the sortable fields.</typeparam>
    [DebuggerDisplay("Order by {Field} {Order}")]
    public sealed class SortSpec<TSortField> where TSortField : struct, IComparable
    {
        public SortSpec(TSortField field, SortOrder order)
        {
            if (!typeof(TSortField).IsEnum)
            {
                throw new ArgumentException(
                    $"Sort field generic parameter ({typeof(TSortField).FullName}) should be an enum.", nameof(field));
            }
            Field = field;
            Order = order;
        }

        public TSortField Field { get; }

        public SortOrder Order { get; }
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }

    public sealed class SortSpecs<TSortField> : Collection<SortSpec<TSortField>>
        where TSortField : struct, IComparable
    {
        public void Add(TSortField field)
        {
            Add(new SortSpec<TSortField>(field, SortOrder.Ascending));
        }

        public void Add(TSortField field, SortOrder order)
        {
            Add(new SortSpec<TSortField>(field, order));
        }

        protected override void InsertItem(int index, SortSpec<TSortField> spec)
        {
            EnforceUniqueFields(spec);
            base.InsertItem(index, spec);
        }

        protected override void SetItem(int index, SortSpec<TSortField> spec)
        {
            EnforceUniqueFields(spec, index);
            base.SetItem(index, spec);
        }

        private void EnforceUniqueFields(SortSpec<TSortField> spec, int replaceIndex = -1)
        {
            for (int i = 0; i < Count; i++)
            {
                if (i != replaceIndex && this[i].Field.Equals(spec.Field))
                    throw new InvalidOperationException($"Cannot insert a sort specification for field {typeof(TSortField)}.{spec.Field} because one already exists.");
            }
        }
    }
}