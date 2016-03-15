using System.Data;
using System.Data.SqlClient;

using Dapper;

namespace Basics.Data.Dapper.Tests
{
    public abstract class TypeHandlerTests<THandler>
        where THandler: SqlMapper.ITypeHandler, new()
    {
        protected SqlMapper.ITypeHandler CreateHandler() => new THandler();

        protected static IDbDataParameter CreateParameter() => new SqlParameter { ParameterName = "@Param" };
    }
}