using System.Collections.Generic;
using System.Dynamic;

namespace Basics.Data.Dapper
{
    public sealed class SearchQuery
    {
        internal SearchQuery(string dataQuery, string countQuery, IDictionary<string, object> parameters)
        {
            DataQuery = dataQuery;
            CountQuery = countQuery;
            Parameters = parameters;
        }

        public string DataQuery { get; }

        public string CountQuery { get; }

        public bool HasCountQuery => CountQuery != null;

        public IDictionary<string, object> Parameters { get; }
    }
}
