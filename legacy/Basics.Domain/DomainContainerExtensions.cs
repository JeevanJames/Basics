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
            TypeInfo baseDomainIntf = typeof(IBaseDomain).GetTypeInfo();
            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<TypeInfo> domainTypes = assembly.ExportedTypes
                    .Select(type => type.GetTypeInfo())
                    .Where(typeInfo => !typeInfo.IsAbstract && typeInfo.IsClass &&
                        baseDomainIntf.IsAssignableFrom(typeInfo));
                foreach (TypeInfo domainType in domainTypes)
                {
                    Type interfaceType =
                        domainType.ImplementedInterfaces.FirstOrDefault(
                            intf => intf.Name.Equals($"I{domainType.Name}", StringComparison.OrdinalIgnoreCase));
                    if (interfaceType != null)
                        builder.RegisterType(interfaceType, domainType.AsType());
                }
            }
        }

        /// <summary>
        ///     Ensures that the IDomainFactory interface is registered with the container.
        /// </summary>
        /// <param name="builder">The container builder to register the IDomainFactory interface with.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void EnsureDomainSupport(IContainerBuilder builder)
        {
            if (!_domainSupportAdded)
            {
                builder.RegisterType<IDomainFactory, DomainFactory>();
                _domainSupportAdded = true;
            }
        }
    }
}
