using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

using Basics.Data.Dapper.Config;
using Basics.Models;

namespace Basics.Data.Dapper
{
    public abstract partial class SearchQueryBuilder
    {
        private readonly string _baseQuery;
        private string _dataQuery;
        private string _countQuery;

        protected SearchQueryBuilder(string baseQuery, bool hasExistingConditions)
        {
            _baseQuery = baseQuery;
            HasExistingConditions = hasExistingConditions;
        }

        public SearchQuery Build(SearchCriteria criteria, object parameters = null)
        {
            PrepareQueries();
            IDictionary<string, object> parameterDictionary = GetParametersFromObject(parameters);
            return criteria == null
                ? CreateSearchQueryInstance(_dataQuery, null, parameterDictionary)
                : DoBuild(criteria, _dataQuery, _countQuery, parameterDictionary);
        }

        public SearchQuery Build<TSortField>(SearchCriteria<TSortField> criteria, object parameters = null)
            where TSortField : struct, IComparable
        {
            PrepareQueries();
            IDictionary<string, object> parameterDictionary = GetParametersFromObject(parameters);
            return criteria == null
                ? CreateSearchQueryInstance(_dataQuery, null, parameterDictionary)
                : DoBuild(criteria, _dataQuery, _countQuery, parameterDictionary);
        }

        public SearchQuery Build<TFilterField, TSortField>(SearchCriteria<TFilterField, TSortField> criteria,
            object parameters = null)
            where TFilterField : struct, IComparable
            where TSortField : struct, IComparable
        {
            PrepareQueries();
            IDictionary<string, object> parameterDictionary = GetParametersFromObject(parameters);
            return criteria == null
                ? CreateSearchQueryInstance(_dataQuery, null, parameterDictionary)
                : DoBuild(criteria, _dataQuery, _countQuery, parameterDictionary, HasExistingConditions);
        }

        protected bool HasExistingConditions { get; }

        private void PrepareQueries()
        {
            if (_dataQuery == null)
                PrepareQueries(_baseQuery, out _dataQuery, out _countQuery);
        }

        /// <summary>
        /// Creates a data query and count query from the specified base query.
        /// </summary>
        /// <param name="baseQuery">The base query from which to extract the data and count queries. The fields must be surrounded by a [[ ]] pair.</param>
        /// <param name="dataQuery">The extracted data query.</param>
        /// <param name="countQuery">The extracted count query.</param>
        protected virtual void PrepareQueries(string baseQuery, out string dataQuery, out string countQuery)
        {
            dataQuery = FieldPattern.Replace(_baseQuery, match => match.Groups[1].Value, 1);
            countQuery = FieldPattern.Replace(_baseQuery, "COUNT(*)", 1);
        }

        private static readonly Regex FieldPattern = new Regex(@"\[\[(.+)\]\]");

        private static IDictionary<string, object> GetParametersFromObject(object parameters)
        {
            var parameterDictionary = parameters as IDictionary<string, object>;
            if (parameterDictionary != null)
                return parameterDictionary;
            if (parameters == null)
                return new Dictionary<string, object>(0);

            List<KeyValuePair<string, object>> parameterPairs = (
                from pi in parameters.GetType().GetProperties()
                where pi.CanRead && pi.GetIndexParameters().Length == 0
                select new KeyValuePair<string, object>(pi.Name, pi.GetValue(parameters, null))
                ).ToList();
            IDictionary<string, object> result = new Dictionary<string, object>(parameterPairs.Count);
            foreach (KeyValuePair<string, object> parameterPair in parameterPairs)
                result.Add(parameterPair);
            return result;
        }

        protected abstract SearchQuery DoBuild(SearchCriteria criteria, string dataQuery, string countQuery,
            IDictionary<string, object> parameters);

        protected abstract SearchQuery DoBuild<TSortField>(SearchCriteria<TSortField> criteria,
            string dataQuery, string countQuery, IDictionary<string, object> parameters)
            where TSortField : struct, IComparable;

        protected abstract SearchQuery DoBuild<TFilterField, TSortField>(
            SearchCriteria<TFilterField, TSortField> criteria,
            string dataQuery, string countQuery, IDictionary<string, object> parameters, bool hasExistingCondition)
            where TFilterField : struct, IComparable
            where TSortField : struct, IComparable;

        /// <summary>
        ///     Creates a <see cref="SearchQuery" /> instance from the provided data and count queries, after assigning their
        ///     parameter values.
        /// </summary>
        /// <param name="dataQuery">The data query.</param>
        /// <param name="countQuery">The count query.</param>
        /// <param name="parameters">The parameters to the queries.</param>
        /// <returns>An instance of <see cref="SearchQuery" />.</returns>
        protected SearchQuery CreateSearchQueryInstance(string dataQuery, string countQuery,
            IDictionary<string, object> parameters)
        {
            return new SearchQuery(dataQuery, countQuery, parameters);

            //TODO: Do we need to convert to an ExpandoObject? Can't we just pass the dictionary to Dapper?
            //var objectParameters = new ExpandoObject();
            //var parameterDictionary = (IDictionary<string, object>)objectParameters;
            //foreach (KeyValuePair<string, object> parameter in parameters) 
            //    parameterDictionary.Add(parameter);
            //return new SearchQuery(dataQuery, countQuery, objectParameters);
        }
    }

    public abstract partial class SearchQueryBuilder
    {
        /// <summary>
        ///     Factory method to create an instance of <see cref="SearchQueryBuilder" /> from the concrete type specified in
        ///     configuration.
        /// </summary>
        /// <param name="dataQuery">The base data query for the search.</param>
        /// <param name="hasExistingConditions">True, if the base data query has existing WHERE conditions.</param>
        /// <returns>An instance of <see cref="SearchQueryBuilder" />.</returns>
        public static SearchQueryBuilder Create(string dataQuery, bool hasExistingConditions = false)
        {
            Type type = DapperConfig.Current?.SearchQueryBuilder;
            if (type == null)
                throw new ConfigurationErrorsException("Missing configuration for Basics.Data.Dapper.");
            return Activator.CreateInstance(type, dataQuery, hasExistingConditions) as SearchQueryBuilder;
        }
    }
}
