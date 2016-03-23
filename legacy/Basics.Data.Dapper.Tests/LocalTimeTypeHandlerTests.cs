using System;
using System.Data;

using Basics.Data.Dapper.TypeHandlers;

using Dapper;

using NodaTime;

using Xunit;

namespace Basics.Data.Dapper.Tests
{
    public sealed class LocalTimeTypeHandlerTests : TypeHandlerTests<LocalTimeTypeHandler>
    {
        [Theory]
        [InlineData(4, 14, 22)]
        [InlineData(0, 0, 0)]
        [InlineData(23, 59, 59)]
        public void Can_convert_localtime_to_timespan(int hour, int minute, int second)
        {
            SqlMapper.ITypeHandler handler = CreateHandler();
            var localTime = new LocalTime(hour, minute, second);
            IDbDataParameter parameter = CreateParameter();

            handler.SetValue(parameter, localTime);

            Assert.Equal(new TimeSpan(0, hour, minute, second), parameter.Value);
        }

        [Theory]
        [InlineData(4, 14, 22)]
        [InlineData(0, 0, 0)]
        [InlineData(23, 59, 59)]
        [InlineData(24, 59, 59)]
        public void Can_convert_timespan_to_localtime(int hour, int minute, int second)
        {
            var handler = new LocalTimeTypeHandler();
            var timespan = new TimeSpan(0, hour, minute, second);

            LocalTime localTime = handler.Parse(timespan);

            Assert.Equal(new LocalTime(hour, minute, second), localTime);
        }

        [Theory]
        [InlineData(2016, 3, 2, 4, 14, 22)]
        [InlineData(2016, 3, 2, 0, 0, 0)]
        [InlineData(2016, 3, 2, 23, 59, 59)]
        public void Can_convert_datetime_to_localtime(int year, int month, int day, int hour, int minute, int second)
        {
            var handler = new LocalTimeTypeHandler();
            var datetime = new DateTime(year, month, day, hour, minute, second);

            LocalTime localTime = handler.Parse(datetime);

            Assert.Equal(new LocalTime(hour, minute, second), localTime);
        }

        [Theory]
        [InlineData(1)]
        [InlineData("Some String")]
        [InlineData(5.4d)]
        [InlineData(DayOfWeek.Wednesday)]
        public void Should_not_convert_unsupported_types_to_localtime(object unsupportedValue)
        {
            var handler = new LocalTimeTypeHandler();
            Assert.Throws<ArgumentException>(() => handler.Parse(unsupportedValue));
        }
    }
}
