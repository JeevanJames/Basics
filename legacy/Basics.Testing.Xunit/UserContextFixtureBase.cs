using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Basics.Testing.Xunit
{
    /// <summary>
    ///     Base fixture class for sample user context creation.
    /// </summary>
    public abstract class UserContextFixtureBase
    {
        private readonly Dictionary<UserTypes, ClaimsPrincipal> _users = new Dictionary<UserTypes, ClaimsPrincipal>(4);

        public ClaimsPrincipal this[UserTypes userType]
        {
            get
            {
                ClaimsPrincipal user;
                if (!_users.TryGetValue(userType, out user))
                {
                    user = CreateUser(userType);
                    _users.Add(userType, user);
                }
                return user;
            }
        }

        private ClaimsPrincipal CreateUser(UserTypes userType)
        {
            switch (userType)
            {
                case UserTypes.AuthorizedAdmin:
                    return CreateUser(AdminId, GetAdminPermissions().ToArray());
                case UserTypes.UnauthorizedAdmin:
                    return CreateUser(AdminId);
                case UserTypes.AuthorizedUser:
                    return CreateUser(UserId, GetUserPermissions().ToArray());
                case UserTypes.UnauthorizedUser:
                    return CreateUser(UserId);
                default:
                    throw new ArgumentOutOfRangeException(nameof(userType), userType, null);
            }
        }

        public ClaimsPrincipal AuthorizedUser => this[UserTypes.AuthorizedUser];

        public ClaimsPrincipal UnauthorizedUser => this[UserTypes.UnauthorizedUser];

        protected virtual IEnumerable<string> GetUserPermissions() => GetAdminPermissions();

        protected virtual string UserId => "user";

        public ClaimsPrincipal AuthorizedAdmin => this[UserTypes.AuthorizedAdmin];

        public ClaimsPrincipal UnauthorizedAdmin => this[UserTypes.UnauthorizedAdmin];

        protected abstract IEnumerable<string> GetAdminPermissions();

        protected virtual string AdminId => "admin";

        protected ClaimsPrincipal CreateUser(string userId, IEnumerable<string> permissions = null)
        {
            permissions = permissions ?? Enumerable.Empty<string>();
            var permissionClaim = new Claim(BasicsClaimTypes.Permission, string.Empty);
            foreach (string permission in permissions)
                permissionClaim.Properties.Add(permission, null);
            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, userId), permissionClaim
            });
            return new ClaimsPrincipal(identity);
        }
    }

    public enum UserTypes
    {
        AuthorizedAdmin,
        UnauthorizedAdmin,
        AuthorizedUser,
        UnauthorizedUser,
    }
}
