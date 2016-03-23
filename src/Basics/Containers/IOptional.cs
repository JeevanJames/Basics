namespace Basics.Containers
{
    /// <summary>
    ///     Denotes an optional injected dependency.
    /// </summary>
    /// <typeparam name="TContract">The type that the dependency resolves to.</typeparam>
    public interface IOptional<out TContract>
    {
        /// <summary>
        ///     Resolves the optional dependency with the specified key.
        /// </summary>
        /// <param name="key">The key of the dependency.</param>
        /// <returns>An instance of the dependency or null if not registered.</returns>
        TContract Resolve(string key);
    }

    public static class OptionalExtensions
    {
        /// <summary>
        ///     Extends the <see cref="IOptional{TContract}.Resolve" /> method by resolving the default dependency (without key).
        /// </summary>
        /// <typeparam name="TContract">The type of the dependency.</typeparam>
        /// <param name="optional">The instance of <see cref="IOptional{TContract}" /> to extend.</param>
        /// <returns>An instance of the dependency or null if not registered.</returns>
        public static TContract Resolve<TContract>(this IOptional<TContract> optional) => optional.Resolve(null);
    }

    internal sealed class Optional<TContract> : IOptional<TContract>
    {
        TContract IOptional<TContract>.Resolve(string key) => Ioc.Container.ResolveOptional<TContract>(key);
    }
}
