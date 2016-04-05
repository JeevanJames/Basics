using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Basics.Models;

namespace Basics.Testing.Xunit
{
    /// <summary>
    ///     Base fixture class for sample user context creation.
    /// </summary>
    public abstract class UserContextFixtureBase
    {
        private ClaimsPrincipal _authorizedUser;
        private ClaimsPrincipal _unauthorizedUser;
        private ClaimsPrincipal _authorizedAdmin;
        private ClaimsPrincipal _unauthorizedAdmin;

        public ClaimsPrincipal AuthorizedUser =>
            _authorizedUser ?? (_authorizedUser = CreateUser(UserId, GetUserPermissions().ToArray()));

        public ClaimsPrincipal UnauthorizedUser =>
            _unauthorizedUser ?? (_unauthorizedUser = CreateUser(UserId));

        protected virtual IEnumerable<string> GetUserPermissions() => GetAdminPermissions();

        protected virtual string UserId => "user";

        public ClaimsPrincipal AuthorizedAdmin =>
            _authorizedAdmin ?? (_authorizedAdmin = CreateUser(AdminId, GetAdminPermissions().ToArray()));

        public ClaimsPrincipal UnauthorizedAdmin =>
            _unauthorizedAdmin ?? (_unauthorizedAdmin = CreateUser(AdminId));

        protected abstract IEnumerable<string> GetAdminPermissions();

        protected virtual string AdminId => "admin";

        protected ClaimsPrincipal CreateUser(string userId, IEnumerable<string> permissions = null)
        {
            permissions = permissions ?? Enumerable.Empty<string>();
            var permissionClaim = new Claim(BasicsClaimTypes.Permission, string.Empty);
            foreach (string permission in permissions)
                permissionClaim.Properties.Add(permission, null);
            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, userId),
                permissionClaim
            });
            return new ClaimsPrincipal(identity);
        }
    }
}
