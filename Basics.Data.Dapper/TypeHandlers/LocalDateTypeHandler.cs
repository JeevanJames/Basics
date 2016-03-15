using System;
using System.Data;

using Dapper;

using NodaTime;

namespace Basics.Data.Dapper.TypeHandlers
{
    public sealed class LocalDateTypeHandler : SqlMapper.TypeHandler<LocalDate>
    {
        public override void SetValue(IDbDataParameter parameter, LocalDate value)
        {
            parameter.Value = value.AtMidnight().ToDateTimeUnspecified();
            parameter.DbType = DbType.Date;
        }

        public override LocalDate Parse(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value is DateTime)
                return LocalDateTime.FromDateTime((DateTime)value).Date;
            throw new ArgumentException($"Cannot convert {value.GetType().FullName} to a NodaTime LocalDate.", nameof(value));
        }
    }
}