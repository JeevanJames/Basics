using System.Dynamic;

namespace Basics.Data.Dapper
{
    public sealed class SearchQuery
    {
        internal SearchQuery(string dataQuery, string countQuery, ExpandoObject parameters)
        {
            DataQuery = dataQuery;
            CountQuery = countQuery;
            Parameters = parameters;
        }

        public string DataQuery { get; }

        public string CountQuery { get; }

        public bool HasCountQuery => CountQuery != null;

        public ExpandoObject Parameters { get; }
    }
}