using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace Basics.Models
{
    /// <summary>
    ///     Generic representation of a user, their permissions and other metadata.
    /// </summary>
    public sealed class UserContext
    {
        private readonly List<string> _permissions = new List<string>();

        public UserContext(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (userId.Length == 0)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }

        public UserContext(string userId, params string[] permissions) : this(userId)
        {
            _permissions.AddRange(permissions);
        }

        /// <summary>
        ///     Gets the identifier of the user.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        ///     Gets the list of permissions assigned to the user.
        /// </summary>
        public IReadOnlyList<string> Permissions => _permissions;

        /// <summary>
        ///     Checks whether the user has the specified permission. Throws a SecurityException if the user does not have the
        ///     specified permission.
        /// </summary>
        /// <param name="requiredPermission">The permission to check for.</param>
        public void Demand(string requiredPermission)
        {
            if (!_permissions.Any(perm => perm.Equals(requiredPermission, StringComparison.OrdinalIgnoreCase)))
                throw new SecurityException("You do not have permissions to perform this operation.");
        }

        /// <summary>
        ///     Checks whether the user has all the specified permissions. Throws a SecurityException if the user does not have all
        ///     the specified permissions.
        /// </summary>
        /// <param name="requiredPermissions">The list of permissions to check for.</param>
        public void DemandAll(params string[] requiredPermissions)
        {
            if (
                !_permissions.All(
                    perm => requiredPermissions.Any(rp => rp.Equals(perm, StringComparison.OrdinalIgnoreCase))))
                throw new SecurityException("You do not have permissions to perform this operation.");
        }

        /// <summary>
        ///     Checks whether the user has any of the specified permissions. Throws a SecurityException if the user does not have
        ///     any of the specified permissions.
        /// </summary>
        /// <param name="requiredPermissions">The list of permissions to check for.</param>
        public void DemandAny(params string[] requiredPermissions)
        {
            if (
                !_permissions.Any(
                    perm => requiredPermissions.Any(rp => rp.Equals(perm, StringComparison.OrdinalIgnoreCase))))
                throw new SecurityException("You do not have permissions to perform this operation.");
        }
    }
}