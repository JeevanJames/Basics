using System;
using System.Collections.Generic;
using System.Security;
using System.Web.Http.Dependencies;

using Basics.Containers;

namespace Basics.AspNet.WebApi
{
    [SecurityCritical]
    public sealed class BasicsDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;
        private readonly IDependencyScope _rootDependencyScope;
        private bool _isDisposed;

        public BasicsDependencyResolver(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            _container = container;
            _rootDependencyScope = new BasicsDependencyScope(container);
        }

        ~BasicsDependencyResolver()
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
                _rootDependencyScope?.Dispose();
            _isDisposed = true;
        }

        IDependencyScope IDependencyResolver.BeginScope() =>
            new BasicsDependencyScope(_container.CreateScope(ScopeTag));

        object IDependencyScope.GetService(Type serviceType) =>
            _rootDependencyScope.GetService(serviceType);

        IEnumerable<object> IDependencyScope.GetServices(Type serviceType) =>
            _rootDependencyScope.GetServices(serviceType);

        private static readonly object ScopeTag = "BasicsWebRequest";
    }
}