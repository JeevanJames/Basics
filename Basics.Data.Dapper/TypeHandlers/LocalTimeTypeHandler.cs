using System;
using System.Data;

using Dapper;

using NodaTime;

namespace Basics.Data.Dapper.TypeHandlers
{
    public sealed class LocalTimeTypeHandler : SqlMapper.TypeHandler<LocalTime>
    {
        public override void SetValue(IDbDataParameter parameter, LocalTime value)
        {
            parameter.Value = TimeSpan.FromTicks(value.TickOfDay);
            parameter.DbType = DbType.Time;
        }

        public override LocalTime Parse(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value is TimeSpan)
                return LocalTime.FromTicksSinceMidnight(((TimeSpan)value).Ticks);
            if (value is DateTime)
                return LocalTime.FromTicksSinceMidnight(((DateTime)value).TimeOfDay.Ticks);
            throw new ArgumentException($"Cannot convert {value.GetType().FullName} to a NodaTime LocalTime.", nameof(value));
        }
    }
}