using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Basics.Containers;

namespace Basics.Domain
{
    public static class DomainContainerExtensions
    {
        public static void AddDomainSupport(this IContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(IDomainFactory), typeof(DomainFactory));
        }

        public static void RegisterDomains(this IContainerBuilder builder, params Assembly[] assemblies)
        {
            TypeInfo domainFactoryInterface = typeof(IDomainFactory).GetTypeInfo();
            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<TypeInfo> domainTypes = assembly.ExportedTypes
                    .Select(type => type.GetTypeInfo())
                    .Where(typeInfo => !typeInfo.IsAbstract && typeInfo.IsClass &&
                        domainFactoryInterface.IsAssignableFrom(typeInfo));
                foreach (TypeInfo domainType in domainTypes)
                {
                    Type interfaceType = domainType.ImplementedInterfaces.FirstOrDefault(intf => intf.Name.Equals($"I{domainType.Name}", StringComparison.OrdinalIgnoreCase));
                    if (interfaceType != null)
                        builder.RegisterType(interfaceType, domainType.AsType());
                }
            }
        }
    }
}