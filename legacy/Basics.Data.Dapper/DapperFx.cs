using Basics.Data.Dapper.TypeHandlers;

using Dapper;

using NodaTime;

namespace Basics.Data.Dapper
{
    public static class DapperFx
    {
        public static void Initialize()
        {
            SqlMapper.AddTypeHandler<Instant>(new InstantTypeHandler());
            SqlMapper.AddTypeHandler<LocalDate>(new LocalDateTypeHandler());
            SqlMapper.AddTypeHandler<LocalTime>(new LocalTimeTypeHandler());
        }
    }
}