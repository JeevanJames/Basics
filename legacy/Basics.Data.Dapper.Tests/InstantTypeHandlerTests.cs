using System;
using System.Data;

using Basics.Data.Dapper.TypeHandlers;

using Dapper;

using NodaTime;

using Xunit;

namespace Basics.Data.Dapper.Tests
{
    public sealed class InstantTypeHandlerTests : TypeHandlerTests<InstantTypeHandler>
    {
        [Theory]
        [InlineData(2016, 3, 2, 4, 14, 22)]
        [InlineData(2016, 3, 2, 0, 0, 0)]
        [InlineData(2016, 3, 2, 23, 59, 59)]
        public void Can_convert_instant_to_datetime(int year, int month, int day, int hour, int minute, int second)
        {
            SqlMapper.ITypeHandler handler = CreateHandler();
            var dateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
            Instant instant = Instant.FromDateTimeUtc(dateTime);
            IDbDataParameter parameter = CreateParameter();

            handler.SetValue(parameter, instant);

            Assert.Equal(DbType.DateTime2, parameter.DbType);
            Assert.Equal(dateTime, parameter.Value);
            Assert.Equal(DateTimeKind.Utc, dateTime.Kind);
        }

        [Theory]
        [InlineData(2016, 3, 2, 4, 14, 22)]
        [InlineData(2016, 3, 2, 0, 0, 0)]
        [InlineData(2016, 3, 2, 23, 59, 59)]
        public void Can_convert_datetime_to_instant(int year, int month, int day, int hour, int minute, int second)
        {
            SqlMapper.ITypeHandler handler = CreateHandler();
            var dateTimeUtc = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
            var dateTimeLocal = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);
            var dateTimeUnspecified = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);

            var instantUtc = (Instant) handler.Parse(typeof (Instant), dateTimeUtc);
            var instantLocal = (Instant) handler.Parse(typeof (Instant), dateTimeLocal);
            var instantUnspecified = (Instant) handler.Parse(typeof (Instant), dateTimeUnspecified);

            Assert.Equal(Instant.FromDateTimeUtc(dateTimeUtc), instantUtc);
            Assert.Equal(Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTimeLocal, DateTimeKind.Utc)), instantLocal);
            Assert.Equal(Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTimeUnspecified, DateTimeKind.Utc)),
                instantUnspecified);
        }
    }
}
