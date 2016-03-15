using System;

using Autofac;

namespace Basics.Containers.Autofac
{
    public sealed class AutofacContainerBuilder : IContainerBuilder
    {
        private readonly ContainerBuilder _builder = new ContainerBuilder();

        IContainer IContainerBuilder.Build() => new AutofacContainer(_builder.Build());

        void IContainerBuilder.RegisterFactory(Type contractType, Func<object> factory)
        {
            _builder.Register(context => factory()).As(contractType);
        }

        void IContainerBuilder.RegisterFactory<TContract>(Func<TContract> factory)
        {
            _builder.Register(context => factory()).As<TContract>();
        }

        void IContainerBuilder.RegisterGeneric(Type contractType, Type implementerType)
        {
            _builder.RegisterGeneric(implementerType).As(contractType);
        }

        void IContainerBuilder.RegisterInstance(Type contractType, object instance)
        {
            _builder.RegisterInstance(instance).As(contractType).ExternallyOwned();
        }

        void IContainerBuilder.RegisterInstance<TContract>(TContract instance)
        {
            _builder.RegisterInstance((object)instance).As<TContract>().ExternallyOwned();
        }

        void IContainerBuilder.RegisterType(Type contractType, Type implementationType, string key)
        {
            if (key == null)
                _builder.RegisterType(implementationType).As(contractType);
            else
                _builder.RegisterType(implementationType).Named(key, contractType);
        }

        void IContainerBuilder.RegisterType<TContract>(Type implementationType, string key)
        {
            ((IContainerBuilder)this).RegisterType(typeof(TContract), implementationType, key);
        }

        void IContainerBuilder.RegisterType<TContract, TImplementation>(string key)
        {
            ((IContainerBuilder)this).RegisterType(typeof(TContract), typeof(TImplementation), key);
        }

        void IContainerBuilder.RegisterTypeAsSelf(Type type)
        {
            _builder.RegisterType(type).AsSelf();
        }

        void IContainerBuilder.RegisterTypeAsSelf<TType>()
        {
            _builder.RegisterType(typeof(TType)).AsSelf();
        }

        public ContainerBuilder Builder => _builder;
    }
}