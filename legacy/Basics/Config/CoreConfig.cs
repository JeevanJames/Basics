using System;

namespace Basics.Config
{
    public sealed class CoreConfig
    {
        static CoreConfig()
        {
#if NETFX
            var section = (CoreSection)System.Configuration.ConfigurationManager.GetSection("basics/core");
            Config = new CoreConfig {
                Container = new ContainerConfig(section.Container.Builder) {
                    AuditorType = section.Container.Auditor?.Type,
                    CacheType = section.Container.Cache?.Type,
                    DistributedCacheType = section.Container.DistributedCache?.Type,
                    LoggerType = section.Container.Logger?.Type,
                }
            };
#else
            Config = new CoreConfig {
                Container =
                    new ContainerConfig(
                        Type.GetType("Basics.Containers.Autofac.AutofacContainerBuilder, Basics.Containers.Autofac"))
            };
#endif
        }

        public static CoreConfig Config { get; }

        public ContainerConfig Container { get; private set; }
    }

    public sealed class ContainerConfig
    {
        internal ContainerConfig(Type builderType)
        {
            BuilderType = builderType;
        }

        public Type BuilderType { get; }

        public Type AuditorType { get; internal set; }
        public Type CacheType { get; internal set; }
        public Type DistributedCacheType { get; internal set; }
        public Type LoggerType { get; internal set; }
    }
}