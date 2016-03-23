using System;

using Basics.Containers;

namespace Basics.Testing.Xunit
{
    /// <summary>
    ///     Base fixture class for dependency injection container initialization.
    ///     Derive from this class and call the base constructor with the appropriate parameters to initialize the container.
    /// </summary>
    public abstract class ContainerFixtureBase
    {
        /// <summary>
        ///     Creates an instance of the container fixture.
        ///     Deriving classes should have a parameterless constructor that sets both the base constructor parameters.
        /// </summary>
        /// <param name="registration">Function that accepts the container builder and registers all dependencies.</param>
        /// <param name="builder">
        ///     Optional container builder object. If not specified, the builder is instantiated from
        ///     configuration using the ContainerFactory class.
        /// </param>
        protected ContainerFixtureBase(Action<IContainerBuilder> registration, IContainerBuilder builder = null)
        {
            if (builder == null)
                builder = Ioc.CreateBuilder();
            registration(builder);
            Container = Ioc.CreateContainer(builder);
        }

        /// <summary>
        ///     Gets the instance of the container.
        /// </summary>
        public IContainer Container { get; }
    }
}