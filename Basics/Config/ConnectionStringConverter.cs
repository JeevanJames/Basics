using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;

namespace Basics.Config
{
    public sealed class ConnectionStringConverter : ConfigurationConverterBase
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var settings = (ConnectionStringSettings)value;
            return settings.Name;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var connectionStringName = value as string;
            if (connectionStringName == null)
                return null;
            return ConfigurationManager.ConnectionStrings[connectionStringName];
        }
    }
}