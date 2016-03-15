using Xunit;

namespace Basics.Testing.Xunit
{
    /// <summary>
    ///     Base class for tests that require access to different types of user contexts, both valid and invalid.
    ///     This class derives from the BaseTests class, meaning that it also has access to the dependency injection container.
    /// </summary>
    /// <typeparam name="TContainerFixture">Implementation of the container fixture where the container is initialized.</typeparam>
    /// <typeparam name="TUserContextFixture">
    ///     Implementation of the user context fixture where the sample user contexts are
    ///     initialized.
    /// </typeparam>
    public abstract class BaseUserContextTest<TContainerFixture, TUserContextFixture> : BaseTest<TContainerFixture>,
        IClassFixture<TUserContextFixture>
        where TContainerFixture : ContainerFixtureBase
        where TUserContextFixture : UserContextFixtureBase
    {
        protected BaseUserContextTest(TContainerFixture containerFixture, TUserContextFixture userContextFixture)
            : base(containerFixture)
        {
            User = userContextFixture;
        }

        /// <summary>
        ///     Gets the sample user data.
        /// </summary>
        public TUserContextFixture User { get; }
    }
}