using Basics.Models;

namespace Basics.Domain
{
    public interface IBaseDomain
    {
        UserContext User { get; }
    }

    public abstract class BaseDomain
    {
        public UserContext User { get; set; }
    }
}
