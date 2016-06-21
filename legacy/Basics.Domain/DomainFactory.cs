using System.Security.Claims;

using Basics.Containers;

namespace Basics.Domain
{
    public interface IDomainFactory
    {
        TDomain Get<TDomain>(ClaimsPrincipal user);
    }

    internal sealed class DomainFactory : IDomainFactory
    {
        TDomain IDomainFactory.Get<TDomain>(ClaimsPrincipal user)
        {
            var domain = Ioc.Container.Resolve<TDomain>();
            var baseDomain = domain as IBaseDomain;
            if (baseDomain == null)
                throw new InvalidDomainException(typeof(TDomain));
            baseDomain.User = user;
            return domain;
        }
    }

    public interface IDomain<out TContract>
    {
        TContract Get(ClaimsPrincipal user);
    }

    internal sealed class Domain<TContract> : IDomain<TContract>
    {
        TContract IDomain<TContract>.Get(ClaimsPrincipal user)
        {
            var domain = Ioc.Container.Resolve<TContract>();
            var baseDomain = domain as IBaseDomain;
            if (baseDomain == null)
                throw new InvalidDomainException(typeof(TContract));
            baseDomain.User = user;
            return domain;
        }
    }
}