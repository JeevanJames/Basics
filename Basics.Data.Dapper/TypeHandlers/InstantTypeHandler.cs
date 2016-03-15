using System;
using System.Data;

using Dapper;

using NodaTime;

namespace Basics.Data.Dapper.TypeHandlers
{
    public sealed class InstantTypeHandler : SqlMapper.TypeHandler<Instant>
    {
        public override void SetValue(IDbDataParameter parameter, Instant value)
        {
            parameter.Value = value.ToDateTimeUtc();
            parameter.DbType = DbType.DateTime2;
        }

        public override Instant Parse(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value is DateTime)
                return Instant.FromDateTimeUtc(DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc));
            if (value is DateTimeOffset)
                return Instant.FromDateTimeOffset((DateTimeOffset)value);
            throw new ArgumentException($"Cannot convert {value.GetType().FullName} to a NodaTime Instant.", nameof(value));
        }
    }
}