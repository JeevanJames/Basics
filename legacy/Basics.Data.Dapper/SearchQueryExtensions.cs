using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Basics.Models;

using Dapper;

namespace Basics.Data.Dapper
{
    public static class SearchQueryExtensions
    {
        public static async Task<TSearchResults> SearchAsync<T, TSearchResults>(this IDbConnection connection,
            SearchQuery searchQuery)
            where TSearchResults : SearchResults<T>
        {
            List<T> permissions = (await connection.QueryAsync<T>(searchQuery.DataQuery, searchQuery.Parameters)
                .ConfigureAwait(false)).ToList();
            int totalCount = searchQuery.HasCountQuery ?
                await connection.ExecuteScalarAsync<int>(searchQuery.CountQuery, searchQuery.Parameters).ConfigureAwait(false) :
                permissions.Count;

            return (TSearchResults)Activator.CreateInstance(typeof(TSearchResults), permissions, totalCount);
        }
    }
}