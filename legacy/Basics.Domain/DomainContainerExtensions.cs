using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using Basics.Containers;

namespace Basics.Domain
{
    public static class DomainContainerExtensions
    {
        private static bool _domainSupportAdded;

        /// <summary>
        ///     Registers all classes in the specified assemblies, which implement the <see cref="IBaseDomain" /> interface, with
        ///     the container.
        /// </summary>
        /// <param name="builder">The container builder to register the domain classes with.</param>
        /// <param name="assemblies">The collection of assemblies to search through for domain classes.</param>
        public static void RegisterDomains(this IContainerBuilder builder, params Assembly[] assemblies)
        {
            EnsureDomainSupport(builder);
            Type baseDomainInterface = typeof(IBaseDomain);
            builder.RegisterByConvention(assemblies, type => baseDomainInterface.IsAssignableFrom(type));
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
