using Basics.Models;

namespace Basics.Domain
{
    public interface IBaseDomain
    {
        UserContext User { get; set; }
    }

    public abstract class BaseDomain
    {
        public UserContext User { get; set; }
    }
}
