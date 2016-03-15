using System;
using System.Collections.Generic;
using System.Text;

using Basics.Containers;
using Basics.Models;

namespace Basics.Data.Dapper.SqlServer
{
    public sealed class SqlServerSearchQueryBuilder : SearchQueryBuilder
    {
        public SqlServerSearchQueryBuilder(string dataQuery, bool hasExistingConditions = false)
            : base(dataQuery, hasExistingConditions)
        {
        }

        protected override SearchQuery DoBuild(SearchCriteria criteria, string dataQuery, string countQuery, IDictionary<string, object> parameters, bool hasExistingConditions)
        {
            bool requiresPagination = criteria.RequiresPagination();

            var dataQueryBuilder = new StringBuilder(dataQuery);
            StringBuilder countQueryBuilder = requiresPagination ? new StringBuilder(countQuery) : null;

            //In SQL Server 2012+, to do pagination with OFFSET and FETCH NEXT, there must be at
            //least one ORDER BY clause.
            //If there are no sort specs, we add one using the first enum value in TSortField and
            //ascending order.
            if (requiresPagination)
            {
                AppendSpaceIfNeeded(dataQueryBuilder);
                dataQueryBuilder.Append($"ORDER BY 1 ASC OFFSET {criteria.StartRecord.GetValueOrDefault(0)} ROWS");
                if (criteria.RecordCount.HasValue)
                    dataQueryBuilder.Append($" FETCH NEXT {criteria.RecordCount.Value} ROWS ONLY");
            }

            return CreateSearchQueryInstance(dataQueryBuilder.ToString(), countQueryBuilder?.ToString(), parameters);
        }

        protected override SearchQuery DoBuild<TSortField>(SearchCriteria<TSortField> criteria, string dataQuery, string countQuery, IDictionary<string, object> parameters,
            bool hasExistingConditions)
        {
            bool requiresPagination = criteria.RequiresPagination();

            var dataQueryBuilder = new StringBuilder(dataQuery);
            StringBuilder countQueryBuilder = requiresPagination ? new StringBuilder(countQuery) : null;

            //In SQL Server 2012+, to do pagination with OFFSET and FETCH NEXT, there must be at
            //least one ORDER BY clause.
            //If there are no sort specs, we add one using the first enum value in TSortField and
            //ascending order.
            if (requiresPagination && criteria.SortSpecs.Count == 0)
            {
                var sortField = (TSortField)Enum.GetValues(typeof(TSortField)).GetValue(0);
                criteria.SortSpecs.Add(sortField, SortOrder.Ascending);
            }

            if (criteria.SortSpecs.Count > 0)
            {
                AppendSpaceIfNeeded(dataQueryBuilder);
                dataQueryBuilder.Append("ORDER BY ");
                for (var i = 0; i < criteria.SortSpecs.Count; i++)
                {
                    if (i > 0)
                        dataQueryBuilder.Append(", ");
                    SortSpec<TSortField> sortSpec = criteria.SortSpecs[i];
                    //TODO:
                    //dataQueryBuilder.Append(
                    //    SearchCriteria<TFilterField, TSortField>.SortFields[sortSpec.Field].ColumnName);
                    dataQueryBuilder.Append(sortSpec.Order == SortOrder.Ascending ? " ASC" : " DESC");
                }
            }

            if (requiresPagination)
            {
                AppendSpaceIfNeeded(dataQueryBuilder);
                dataQueryBuilder.Append($"OFFSET {criteria.StartRecord.GetValueOrDefault(0)} ROWS");
                if (criteria.RecordCount.HasValue)
                    dataQueryBuilder.Append($" FETCH NEXT {criteria.RecordCount.Value} ROWS ONLY");
            }

            return CreateSearchQueryInstance(dataQueryBuilder.ToString(), countQueryBuilder?.ToString(), parameters);
        }

        protected override SearchQuery DoBuild<TFilterField, TSortField>(SearchCriteria<TFilterField, TSortField> criteria, string dataQuery, string countQuery,
            IDictionary<string, object> parameters, bool hasExistingConditions)
        {
            bool requiresPagination = criteria.RequiresPagination();

            var dataQueryBuilder = new StringBuilder(dataQuery);
            StringBuilder countQueryBuilder = requiresPagination ? new StringBuilder(countQuery) : null;

            if (criteria.RequiresFiltering())
            {
                string filterClause = BuildFilterClause(criteria, parameters);
                if (!string.IsNullOrWhiteSpace(filterClause))
                {
                    AppendSpaceIfNeeded(dataQueryBuilder, countQueryBuilder);
                    Append(hasExistingConditions ? "AND" : "WHERE", dataQueryBuilder, countQueryBuilder);
                    AppendSpaceIfNeeded(dataQueryBuilder, countQueryBuilder);
                    Append(filterClause, dataQueryBuilder, countQueryBuilder);
                }
            }

            //In SQL Server 2012+, to do pagination with OFFSET and FETCH NEXT, there must be at
            //least one ORDER BY clause.
            //If there are no sort specs, we add one using the first enum value in TSortField and
            //ascending order.
            if (requiresPagination && criteria.SortSpecs.Count == 0)
            {
                var sortField = (TSortField)Enum.GetValues(typeof(TSortField)).GetValue(0);
                criteria.SortSpecs.Add(sortField, SortOrder.Ascending);
            }

            if (criteria.SortSpecs.Count > 0)
            {
                AppendSpaceIfNeeded(dataQueryBuilder);
                dataQueryBuilder.Append("ORDER BY ");
                for (var i = 0; i < criteria.SortSpecs.Count; i++)
                {
                    if (i > 0)
                        dataQueryBuilder.Append(", ");
                    SortSpec<TSortField> sortSpec = criteria.SortSpecs[i];
                    //TODO:
                    //dataQueryBuilder.Append(
                    //    SearchCriteria<TFilterField, TSortField>.SortFields[sortSpec.Field].ColumnName);
                    dataQueryBuilder.Append(sortSpec.Order == SortOrder.Ascending ? " ASC" : " DESC");
                }
            }

            if (requiresPagination)
            {
                AppendSpaceIfNeeded(dataQueryBuilder);
                dataQueryBuilder.Append($"OFFSET {criteria.StartRecord.GetValueOrDefault(0)} ROWS");
                if (criteria.RecordCount.HasValue)
                    dataQueryBuilder.Append($" FETCH NEXT {criteria.RecordCount.Value} ROWS ONLY");
            }

            return CreateSearchQueryInstance(dataQueryBuilder.ToString(), countQueryBuilder?.ToString(), parameters);
        }

        private static string BuildFilterClause<TFilterField, TSortField>(SearchCriteria<TFilterField, TSortField> criteria,
            IDictionary<string, object> parameters)
            where TFilterField : struct, IComparable
            where TSortField : struct, IComparable
        {
            var clause = new StringBuilder();
            for (var i = 0; i < criteria.Filters.Count; i++)
            {
                FilterCriteria<TFilterField> filter = criteria.Filters[i];
                FieldDefinition fieldDefinition = Ioc.Container.Resolve<IEnumFieldMappings>().Of(filter.Field);
                string columnName = fieldDefinition.ColumnName;
                string operation = GetOperation(filter.Operation);

                if (clause.Length > 0)
                    clause.Append(" AND ");
                clause.Append($"{columnName} {operation} @p{i}");

                parameters.Add($"@p{i}", filter.Value);
            }
            return clause.ToString();
        }

        private static string GetOperation(FilterOperation operation)
        {
            switch (operation)
            {
                case FilterOperation.Equals:
                    return "=";
                case FilterOperation.DoesNotEqual:
                    return "<>";
                case FilterOperation.GreaterThan:
                    return ">";
                case FilterOperation.GreaterThanOrEqual:
                    return ">=";
                case FilterOperation.LessThan:
                    return "<";
                case FilterOperation.LessThanOrEqual:
                    return "<=";
                case FilterOperation.Like:
                    return "LIKE";
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
        }

        private static void Append(string text, StringBuilder dataBuilder, StringBuilder countBuilder = null)
        {
            dataBuilder.Append(text);
            countBuilder?.Append(text);
        }

        private static void AppendSpaceIfNeeded(StringBuilder dataBuilder, StringBuilder countBuilder = null)
        {
            if (dataBuilder[dataBuilder.Length - 1] != ' ')
                dataBuilder.Append(' ');
            if (countBuilder != null && countBuilder[countBuilder.Length - 1] != ' ')
                countBuilder.Append(' ');
        }
    }
}