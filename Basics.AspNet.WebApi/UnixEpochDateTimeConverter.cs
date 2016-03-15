using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Basics.AspNet.WebApi
{
    /// <summary>
    ///     JSON.NET date/time converter to convert .NET DateTime values to and from UNIX epoch values.
    ///     UNIX epoch time is the number of milliseconds since Jan 1, 1970 00:00. It is one of the
    ///     native formats supported by the JavaScript Date object.
    /// </summary>
    public sealed class UnixEpochDateTimeConverter : DateTimeConverterBase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var unixTime = (long)reader.Value;
            DateTime dateTime = _epoch.AddMilliseconds(unixTime);
            return dateTime;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dateTime = (DateTime)value;
            long unixTime = Convert.ToInt64((dateTime - _epoch).TotalMilliseconds);
            writer.WriteValue(unixTime);
        }
    }
}