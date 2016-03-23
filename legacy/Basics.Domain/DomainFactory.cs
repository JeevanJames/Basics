using Basics.Containers;

namespace Basics.Domain
{
    public interface IDomainFactory
    {
        TDomain Get<TDomain>()
            where TDomain : IBaseDomain;
    }

    internal sealed class DomainFactory : IDomainFactory
    {
        TDomain IDomainFactory.Get<TDomain>() =>
            Ioc.Container.Resolve<TDomain>();
    }
}