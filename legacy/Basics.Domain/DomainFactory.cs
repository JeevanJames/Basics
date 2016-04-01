using Basics.Containers;
using Basics.Models;

namespace Basics.Domain
{
    public interface IDomainFactory
    {
        TDomain Get<TDomain>(UserContext user)
            where TDomain : IBaseDomain;
    }

    internal sealed class DomainFactory : IDomainFactory
    {
        TDomain IDomainFactory.Get<TDomain>(UserContext user)
        {
            var domain = Ioc.Container.Resolve<TDomain>();
            domain.User = user;
            return domain;
        }
    }
}