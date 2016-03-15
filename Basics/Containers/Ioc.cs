using System;

using Basics.Caching;
using Basics.Config;
using Basics.Logging;

namespace Basics.Containers
{
    /// <summary>
    ///     Provides methods to create a container builder from configuration and to initialize a builder with default
    ///     registrations.
    /// </summary>
    public static class Ioc
    {
        /// <summary>
        ///     Global access to the dependency injection container from within this assembly.
        /// </summary>
        //TODO: Make this internal
        public static IContainer Container { get; set; }

        /// <summary>
        ///     Creates an instance of the container builder from configuration and initializes it with the default registrations.
        /// </summary>
        /// <returns>The instance of the container builder.</returns>
        public static IContainerBuilder CreateBuilder()
        {
            var builder = (IContainerBuilder) Activator.CreateInstance(CoreConfig.Config.Container.Builder);
            InitializeBuilder(builder);
            return builder;
        }

        /// <summary>
        ///     Initializes the given container builder with the default registrations, all of which are specified in
        ///     configuration.
        /// </summary>
        /// <param name="builder">The container builder to initialize.</param>
        public static void InitializeBuilder(IContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            CoreSection.ContainerElement containerConfig = CoreConfig.Config.Container;
            builder.RegisterType<IAuditor>(containerConfig.Auditor?.Type ?? typeof (NullAuditor));
            builder.RegisterType<ICache>(containerConfig.Cache?.Type ?? typeof (NullCache));
            builder.RegisterType<IDistributedCache>(containerConfig.DistributedCache?.Type ?? typeof (NullCache));
            builder.RegisterType<ILogger>(containerConfig.Logger?.Type ?? typeof (NullLogger));
        }

        /// <summary>
        ///     Builds a container from the specified container builder instance.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <returns>The container built from the container builder.</returns>
        public static IContainer CreateContainer(IContainerBuilder builder)
        {
            //Registers the two special modifiers for injected dependencies - one for keyed
            //dependencies and the other for optional ones.
            builder.RegisterGeneric(typeof (IKeyed<>), typeof (Keyed<>));
            builder.RegisterGeneric(typeof (IOptional<>), typeof (Optional<>));

            IContainer container = builder.Build();
            Container = container;
            return container;
        }
    }
}
