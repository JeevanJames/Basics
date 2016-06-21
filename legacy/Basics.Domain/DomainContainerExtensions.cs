using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Basics.Containers;

namespace Basics.Domain
{
    public static class DomainContainerExtensions
    {
        private static bool _domainSupportAdded;

        public static void RegisterDomains(this IContainerBuilder builder, params Assembly[] assemblies)
        {
            EnsureDomainSupport(builder);
            builder.RegisterByConvention(assemblies, type => type.Name.EndsWith("Domain"));
        }

        /// <summary>
        ///     Ensures that the IDomainFactory and IDomain interfaces are registered with the container.
        /// </summary>
        /// <param name="builder">The container builder to register the IDomainFactory interface with.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void EnsureDomainSupport(IContainerBuilder builder)
        {
            if (!_domainSupportAdded)
            {
                builder.RegisterType<IDomainFactory, DomainFactory>();
                builder.RegisterGeneric(typeof(IDomain<>), typeof(Domain<>));
                _domainSupportAdded = true;
            }
        }
    }
}
