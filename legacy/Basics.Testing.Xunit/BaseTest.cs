using Basics.Containers;

using Xunit;

namespace Basics.Testing.Xunit
{
    /// <summary>
    ///     Base class for tests that require access to the dependency injection container.
    /// </summary>
    /// <typeparam name="TContainerFixture">Implementation of the container fixture where the container is initialized.</typeparam>
    public abstract class BaseTest<TContainerFixture> : IClassFixture<TContainerFixture>
        where TContainerFixture : ContainerFixtureBase
    {
        protected BaseTest(TContainerFixture containerFixture)
        {
            Container = containerFixture.Container;
        }

        /// <summary>
        ///     Gets the instance of the container.
        /// </summary>
        public IContainer Container { get; }
    }
}