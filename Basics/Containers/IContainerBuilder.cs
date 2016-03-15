using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            IEnumerable<Type> typesToRegister = assembly.GetExportedTypes().Where(predicate);
            foreach (Type typeToRegister in typesToRegister)
                builder.RegisterTypeAsSelf(typeToRegister);
        }
    }
}