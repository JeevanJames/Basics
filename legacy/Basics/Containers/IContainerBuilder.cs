using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Basics.Containers
{
    public interface IContainerBuilder
    {
        /// <summary>
        ///     Creates a new dependency injection container with the registrations that have already
        ///     been made.
        /// </summary>
        /// <returns>A new container with the configured service registrations.</returns>
        IContainer Build();

        void RegisterFactory(Type contractType, Func<object> factory);

        void RegisterFactory<TContract>(Func<TContract> factory);

        void RegisterGeneric(Type contractType, Type implementerType);

        void RegisterInstance(Type contractType, object instance);

        void RegisterInstance<TContract>(TContract instance);

        void RegisterType(Type contractType, Type implementationType, string key = null);

        void RegisterType<TContract>(Type implementationType, string key = null);

        void RegisterType<TContract, TImplementation>(string key = null)
            where TImplementation : class, TContract;

        void RegisterTypeAsSelf(Type type);

        void RegisterTypeAsSelf<TType>();
    }

    public static class ContainerBuilderExtensions
    {
        public static void RegisterAssembly(this IContainerBuilder builder, Assembly assembly,
            Func<Type, bool> predicate)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            predicate = predicate ?? (type => true);
            IEnumerable<Type> typesToRegister = assembly.ExportedTypes.Where(predicate);
            foreach (Type typeToRegister in typesToRegister)
                builder.RegisterTypeAsSelf(typeToRegister);
        }

        public static IEnumerable<KeyValuePair<Type, Type>> RegisterByConvention(this IContainerBuilder builder, IEnumerable<Assembly> assemblies,
            Func<Type, bool> predicate = null)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            IEnumerable<Type> allTypes = assemblies.SelectMany(asm => asm.GetExportedTypes());
            predicate = predicate ?? (type => true);
            IEnumerable<Type> matchingClasses = allTypes.Where(type => type.IsClass && predicate(type));
            List<Type> allInterfaces = allTypes.Where(type => type.IsInterface && type.Name.StartsWith("I")).ToList();
            foreach (Type matchingClass in matchingClasses)
            {
                string expectedInterfaceName = $"{matchingClass.Namespace}.I{matchingClass.Name}";
                Type matchingInterfaceType = allInterfaces.FirstOrDefault(type => type.FullName.Equals(expectedInterfaceName, StringComparison.Ordinal));
                if (matchingInterfaceType == null)
                    continue;
                if (matchingInterfaceType.IsAssignableFrom(matchingClass))
                {
                    builder.RegisterType(matchingInterfaceType, matchingClass);
                    yield return new KeyValuePair<Type, Type>(matchingInterfaceType, matchingClass);
                }
            }
        }

        public static IEnumerable<KeyValuePair<Type, Type>> RegisterByConvention(this IContainerBuilder builder, IEnumerable<Assembly> assemblies, Regex classNamePattern)
        {
            if (classNamePattern == null)
                throw new ArgumentNullException(nameof(classNamePattern));
            return RegisterByConvention(builder, assemblies, type => classNamePattern.IsMatch(type.FullName));
        }
    }
}