using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;

namespace Basics
{
    public static class BasicsClaimTypes
    {
        public const string ClaimTypeNamespace = "https://www.nuget.org/packages/Basics/ws/2015/12/identity/claims";

        public const string Permission = ClaimTypeNamespace + "/permissions";
    }

    public static class SecurityExtensions
    {
        public static void Demand(this ClaimsPrincipal principal, string requiredPermission)
        {
            Demand(principal, perms => perms.ContainsKey(requiredPermission));
        }

        public static void DemandAny(this ClaimsPrincipal principal, params string[] requiredPermissions)
        {
            Demand(principal, perms => perms.Any(perm => requiredPermissions.Any(rp => rp.Equals(perm.Key, StringComparison.Ordinal))));
        }

        public static void DemandAll(this ClaimsPrincipal principal, params string[] requiredPermissions)
        {
            Demand(principal, perms => perms.All(perm => requiredPermissions.Any(rp => rp.Equals(perm.Key, StringComparison.Ordinal))));
        }

        private static void Demand(ClaimsPrincipal principal, Func<IDictionary<string, string>, bool> predicate)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            Claim permissionClaim = principal.FindFirst(BasicsClaimTypes.Permission);
            if (permissionClaim == null)
                throw new SecurityException("You do not have permissions to perform this operation.");
            if (!predicate(permissionClaim.Properties))
                throw new SecurityException("You do not have permissions to perform this operation.");
        }

        public static void AddPermissions(this Claim claim, IEnumerable<string> permissions)
        {
            foreach (string permission in permissions)
                claim.Properties.Add(permission, string.Empty);
        }

        /// <summary>
        /// Adds one or more permission claims to the given identity.
        /// </summary>
        /// <param name="identity">The <see cref="ClaimsIdentity"/> instance to add the permission claims to.</param>
        /// <param name="permissions">The permissions to add.</param>
        /// <returns>Instance of the permissions claim</returns>
        public static Claim AddPermissions(this ClaimsIdentity identity, IEnumerable<string> permissions)
        {
            Claim permissionClaim = identity.FindFirst(BasicsClaimTypes.Permission);
            if (permissionClaim == null)
            {
                permissionClaim = new Claim(BasicsClaimTypes.Permission, string.Empty);
                identity.AddClaim(permissionClaim);
            }
            permissionClaim.AddPermissions(permissions);
            return permissionClaim;
        }
    }
}