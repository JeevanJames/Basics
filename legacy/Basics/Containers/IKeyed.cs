namespace Basics.Containers
{
    /// <summary>
    ///     Denotes a named dependency
    /// </summary>
    /// <typeparam name="TContract">The contract of the dependency.</typeparam>
    public interface IKeyed<out TContract>
    {
        TContract this[string key] { get; }
    }

    internal sealed class Keyed<TContract> : IKeyed<TContract>
    {
        TContract IKeyed<TContract>.this[string key] =>
            Ioc.Container.Resolve<TContract>(key);
    }
}