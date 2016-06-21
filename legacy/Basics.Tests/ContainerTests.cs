using System;
using System.Linq;
using System.Reflection;

using Basics.Containers;

using Xunit;

namespace Basics.Tests
{
    public sealed class ContainerTests : IDisposable
    {
        private readonly IContainer _container;

        public ContainerTests()
        {
            IContainerBuilder builder = Ioc.CreateBuilder();
            builder.RegisterByConvention(new[] { Assembly.GetExecutingAssembly() }, type => type.Name == "Process");
            //builder.RegisterType<IProcess, Process>();
            builder.RegisterType<IPerson, Customer>("Customer");
            builder.RegisterType<IPerson, Vendor>("Vendor");
            builder.RegisterGeneric(typeof(IRepository<>), typeof(Repository<>));
            builder.RegisterTypeAsSelf<OptionalTest>();
            _container = Ioc.CreateContainer(builder);
        }

        void IDisposable.Dispose()
        {
            _container.Dispose();
        }

        [Fact]
        public void Process_gets_the_correct_person()
        {
            var process = _container.Resolve<IProcess>();
            Assert.Equal(Vendor.Description, process.DescribePerson());
        }

        [Fact]
        public void Dont_need_key_to_resolve_process()
        {
            Assert.ThrowsAny<Exception>(() => _container.Resolve<IProcess>("SomeKey"));
        }

        [Theory, CLSCompliant(false)]
        [InlineData("Customer", typeof(Customer))]
        [InlineData("Vendor", typeof(Vendor))]
        public void Person_resolves_correctly_based_on_key(string key, Type expectedType)
        {
            var person = _container.Resolve<IPerson>(key);
            Assert.IsType(expectedType, person);
        }

        [Fact]
        public void Need_key_to_resolve_person()
        {
            Assert.ThrowsAny<Exception>(() => _container.Resolve<IPerson>());
        }

        [Fact]
        public void Missing_optional_dependencies_resolve_to_null()
        {
            var optionalTest = _container.ResolveOptional<OptionalTest>();
            Assert.Null(optionalTest.OptionalContract);
        }
    }
}