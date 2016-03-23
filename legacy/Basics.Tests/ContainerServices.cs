using System.Collections.Generic;

using Basics.Containers;

namespace Basics.Tests
{
    public interface IProcess
    {
        string DescribePerson();
    }

    public sealed class Process : IProcess
    {
        private readonly IKeyed<IPerson> _vendor;
        private readonly IRepository<Customer> _repository;

        public Process(IKeyed<IPerson> vendor, IRepository<Customer> repository)
        {
            _vendor = vendor;
            _repository = repository;
        }

        string IProcess.DescribePerson() => _vendor["Vendor"].Describe();
    }

    public interface IPerson
    {
        string Describe();
    }

    public sealed class Customer : IPerson
    {
        public const string Description = "I'm a customer";

        string IPerson.Describe() => Description;
    }

    public sealed class Vendor : IPerson
    {
        public const string Description = "I'm a vendor";

        string IPerson.Describe() => Description;
    }

    public interface IRepository<out T>
    {
        IEnumerable<string> GetList();
    }

    public class Repository<T> : IRepository<T>
    {
        IEnumerable<string> IRepository<T>.GetList()
        {
            yield return typeof(T).FullName;
        }
    }

    public class OptionalTest
    {
        public IOptionalContract OptionalContract { get; }

        public OptionalTest(IOptional<IOptionalContract> optionalContract)
        {
            OptionalContract = optionalContract.Resolve();
        }
    }

    public interface IOptionalContract
    {
    }
}