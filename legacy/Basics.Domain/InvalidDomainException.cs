using System;

namespace Basics.Domain
{
    public sealed class InvalidDomainException : Exception
    {
        public InvalidDomainException(Type type) : base($"{type.FullName} is not a valid domain class as it does not implement {typeof(IBaseDomain).AssemblyQualifiedName}")
        {
        }
    }
}