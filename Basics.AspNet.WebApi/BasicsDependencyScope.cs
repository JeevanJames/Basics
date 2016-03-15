using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

using Basics.Containers;

namespace Basics.AspNet.WebApi
{
    public sealed class BasicsDependencyScope : IDependencyScope
    {
        private readonly IContainer _container;
        private bool _isDisposed;

        public BasicsDependencyScope(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            _container = container;
        }

        ~BasicsDependencyScope()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;
            if (disposing)
                _container?.Dispose();
            _isDisposed = true;
        }

        object IDependencyScope.GetService(Type serviceType) =>
            _container.ResolveOptional(serviceType);

        IEnumerable<object> IDependencyScope.GetServices(Type serviceType)
        {
            object value = _container.ResolveOptional(serviceType);
            return value != null ? (IEnumerable<object>)value : Enumerable.Empty<object>();
        }
    }
}