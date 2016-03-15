using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Basics.Models;

using Dapper;

namespace Basics.Data.Dapper
{
    public static class DapperExtensions
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

        /// <summary>
        ///     Extension method to add a parameter to a <see cref="DynamicParameters" /> object, using the fluent syntax.
        /// </summary>
        /// <param name="parameters">The <see cref="DynamicParameters" /> instance.</param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <returns>The same <see cref="DynamicParameters" /> instance.</returns>
        public static DynamicParameters Set(this DynamicParameters parameters, string parameterName, object value = null,
            DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null,
            byte? scale = null)
        {
            parameters.Add(parameterName, value, dbType, direction, size, precision, scale);
            return parameters;
        }
    }
}