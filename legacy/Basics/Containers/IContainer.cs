using System;

namespace Basics.Containers
{
    /// <summary>
    ///     Abstraction for a dependency injection container.
    /// </summary>
    public interface IContainer : IDisposable
    {
        /// <summary>
        ///     Reference to the underlying DI framework's container instance.
        ///     It is recommended not to use this property as it breaks the framework-independent nature of this system.
        /// </summary>
        object NativeContainer { get; }

        /// <summary>
        ///     Creates a (optionally named) nested container scope from the current container instance.
        /// </summary>
        /// <param name="tag">The name of the scoped container.</param>
        /// <returns>The nested <see cref="IContainer" /> scope.</returns>
        IContainer CreateScope(object tag = null);

        /// <summary>
        ///     Resolves an object of the specified type and optionally the specified key.
        /// </summary>
        /// <param name="type">The type of object to resolve.</param>
        /// <param name="key">Optional key under which the type is registered.</param>
        /// <returns>The resolved object.</returns>
        /// <exception cref="ContainerException">
        ///     Thrown if the type cannot be resolved. Typically, the inner exception will be the
        ///     actual exception thrown by the underlying DI framework.
        /// </exception>
        object Resolve(Type type, string key = null);

        /// <summary>
        ///     Resolves an object of the specified type and optionally the specified key. If the object cannot be resolved, no
        ///     exception is thrown.
        /// </summary>
        /// <param name="type">The type of object to resolve.</param>
        /// <param name="key">Optional key under which the type is registered.</param>
        /// <returns>The resolved object or null if it cannot be resolved.</returns>
        object ResolveOptional(Type type, string key = null);
    }

    public static class ContainerExtensions
    {
        public static TContract Resolve<TContract>(this IContainer container, string key = null) =>
            (TContract)container.Resolve(typeof(TContract), key);

        public static TContract ResolveOptional<TContract>(this IContainer container, string key = null)
        {
            object resolved = container.ResolveOptional(typeof(TContract), key);
            return resolved != null ? (TContract)resolved : default(TContract);
        }
    }
}
