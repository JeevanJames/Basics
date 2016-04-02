using System.Security.Claims;

using Basics.Containers;

namespace Basics.Domain
{
    public interface IDomainFactory
    {
        TDomain Get<TDomain>(ClaimsPrincipal user)
            where TDomain : IBaseDomain;
    }

    internal sealed class DomainFactory : IDomainFactory
    {
        TDomain IDomainFactory.Get<TDomain>(ClaimsPrincipal user)
        {
            var domain = Ioc.Container.Resolve<TDomain>();
            domain.User = user;
            return domain;
        }
    }
}