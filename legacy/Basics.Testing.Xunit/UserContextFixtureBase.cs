using System.Collections.Generic;
using System.Linq;

using Basics.Models;

namespace Basics.Testing.Xunit
{
    /// <summary>
    ///     Base fixture class for sample user context creation.
    /// </summary>
    public abstract class UserContextFixtureBase
    {
        private UserContext _authorizedUser;
        private UserContext _unauthorizedUser;
        private UserContext _authorizedAdmin;
        private UserContext _unauthorizedAdmin;

        public UserContext AuthorizedUser =>
            _authorizedUser ?? (_authorizedUser = new UserContext(UserId, GetUserPermissions().ToArray()));

        public UserContext UnauthorizedUser =>
            _unauthorizedUser ?? (_unauthorizedUser = new UserContext(UserId));

        protected virtual IEnumerable<string> GetUserPermissions() => GetAdminPermissions();

        protected virtual string UserId => "user";

        public UserContext AuthorizedAdmin =>
            _authorizedAdmin ?? (_authorizedAdmin = new UserContext(AdminId, GetAdminPermissions().ToArray()));

        public UserContext UnauthorizedAdmin =>
            _unauthorizedAdmin ?? (_unauthorizedAdmin = new UserContext(AdminId));

        protected abstract IEnumerable<string> GetAdminPermissions();

        protected virtual string AdminId => "admin";
    }
}
