using System.Security.Claims;

namespace Basics.Domain
{
    public interface IBaseDomain
    {
        ClaimsPrincipal User { get; set; }
    }

    public abstract class BaseDomain
    {
        public ClaimsPrincipal User { get; set; }
    }
}
