using System;
using System.Collections.Generic;

using Microsoft.Practices.ServiceLocation;

namespace Basics.Containers.ServiceLocator
{
    /// <summary>
    ///     Common service locator implementation for the Basics container.
    /// </summary>
    public sealed class BasicsServiceLocator : ServiceLocatorImplBase
    {
        private readonly IContainer _container;

        internal BasicsServiceLocator(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            _container = container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return _container.Resolve(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            yield return _container.Resolve(serviceType);
        }

        public static void Setup(IContainer container)
        {
            var locator = new BasicsServiceLocator(container);
            Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(() => locator);
        }
    }
}