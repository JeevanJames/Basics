using System;
using System.ComponentModel;
using System.Configuration;

namespace Basics.Data.Dapper.Config
{
    public static class DapperConfig
    {
        static DapperConfig()
        {
            Current = (DapperSection)ConfigurationManager.GetSection("basics/dapper");
        }

        public static DapperSection Current { get; }
    }

    public sealed class DapperSection : ConfigurationSection
    {
        [ConfigurationProperty("search-query-builder", IsRequired = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type SearchQueryBuilder
        {
            get { return (Type)this["search-query-builder"]; }
            set { this["search-query-builder"] = value; }
        }
    }
}