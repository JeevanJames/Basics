using System;

using Autofac;

namespace Basics.Containers.Autofac
{
    public sealed class AutofacContainer : IContainer
    {
        private readonly ILifetimeScope _container;

        internal AutofacContainer(ILifetimeScope container)
        {
            _container = container;
        }

        object IContainer.NativeContainer => _container;

        IContainer IContainer.CreateScope(object tag) =>
            new AutofacContainer(_container.BeginLifetimeScope(tag));

        object IContainer.Resolve(Type type, string key)
        {
            return key == null ? _container.Resolve(type) : _container.ResolveNamed(key, type);
        }

        object IContainer.ResolveOptional(Type type, string key) => _container.ResolveOptional(type);

        void IDisposable.Dispose()
        {
            _container.Dispose();
        }
    }
}