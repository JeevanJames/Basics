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
    }
}