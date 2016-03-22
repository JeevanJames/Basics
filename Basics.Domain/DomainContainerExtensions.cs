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
            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<Type> domainTypes = assembly.ExportedTypes.Where(type => !type.IsAbstract && type.IsClass &&
                    typeof(IDomainFactory).IsAssignableFrom(type));
                foreach (Type domainType in domainTypes)
                {
                    Type interfaceType = domainType.GetInterface($"I{domainType.Name}");
                    if (interfaceType != null)
                        builder.RegisterType(interfaceType, domainType);
                }
            }
        }
    }
}