#if NETFX
using System;
using System.ComponentModel;
using System.Configuration;

namespace Basics.Config
{
    public sealed class CoreSection : ConfigurationSection
    {
        [ConfigurationProperty("container")]
        public ContainerElement Container => (ContainerElement)this["container"];

        public sealed class ContainerElement : ConfigurationElement
        {
            [ConfigurationProperty("builder", IsRequired = true)]
            [TypeConverter(typeof(TypeNameConverter))]
            public Type Builder
            {
                get { return (Type)this["builder"]; }
                set { this["builder"] = value; }
            }

            [ConfigurationProperty("logger")]
            public DependencyTypeElement Logger => (DependencyTypeElement)this["logger"];

            [ConfigurationProperty("auditor")]
            public DependencyTypeElement Auditor => (DependencyTypeElement)this["auditor"];

            [ConfigurationProperty("cache")]
            public DependencyTypeElement Cache => (DependencyTypeElement)this["cache"];

            [ConfigurationProperty("distributed-cache")]
            public DependencyTypeElement DistributedCache => (DependencyTypeElement)this["distributed-cache"];

            public sealed class DependencyTypeElement : ConfigurationElement
            {
                [ConfigurationProperty("type", IsRequired = true)]
                [TypeConverter(typeof(TypeNameConverter))]
                public Type Type
                {
                    get { return (Type)this["type"]; }
                    set { this["type"] = value; }
                }
            }
        }
    }
}
#endif
