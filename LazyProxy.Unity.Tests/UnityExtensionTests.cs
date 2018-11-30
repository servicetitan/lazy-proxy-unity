using System;
using System.Runtime.CompilerServices;
using Moq;
using Unity;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;
using Xunit;
using DependencyAttribute = Unity.Attributes.DependencyAttribute;

[assembly: InternalsVisibleTo("LazyProxy.DynamicTypes")]

namespace LazyProxy.Unity.Tests
{
    public class UnityExtensionTests
    {
        [ThreadStatic]
        private static string _service1Id;

        [ThreadStatic]
        private static string _service2Id;

        // ReSharper disable once MemberCanBePrivate.Global
        public interface IService1
        {
            string Property { get; set; }
            string MethodWithoutOtherServiceInvocation();
            string MethodWithOtherServiceInvocation(string arg);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Service1 : IService1
        {
            private readonly IService2 _otherService;

            public Service1(IService2 otherService)
            {
                _service1Id = Guid.NewGuid().ToString();
                _otherService = otherService;
            }

            public string Property { get; set; } = "property";
            public string MethodWithoutOtherServiceInvocation() => "service1";
            public string MethodWithOtherServiceInvocation(string arg) => "service1->" + _otherService.Method(arg);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public interface IService2
        {
            string Method(string arg);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Service2 : IService2
        {
            public Service2()
            {
                _service2Id = Guid.NewGuid().ToString();
            }

            public string Method(string arg) => "service2->" + arg;
        }

        private class Service2Ex : IService2
        {
            public string Method(string arg) => "service2Ex->" + arg;
        }

        internal interface IInternalService
        {
            string Get();
        }

        internal class InternalService : IInternalService
        {
            public string Get() => "InternalService";
        }

        public interface IBaseArgument { }

        public abstract class AnotherArgument
        {
            public string Value { get; set; }
        }

        public class Argument1A : IBaseArgument { }

        public class Argument1B : IBaseArgument { }

        public class Argument1C : IBaseArgument { }

        public struct Argument2 { }

        public class Argument3 : AnotherArgument, IBaseArgument { }

        public interface IGenericService<T, in TIn, out TOut>
            where T : class, IBaseArgument, new()
            where TIn : struct
            where TOut : AnotherArgument, IBaseArgument
        {
            TOut Get<TArg>(T arg1, TIn arg2, TArg arg3) where TArg : struct;
        }

        private class GenericService<T, TIn, TOut> : IGenericService<T, TIn, TOut>
            where T : class, IBaseArgument, new()
            where TIn : struct
            where TOut : AnotherArgument, IBaseArgument, new()
        {
            protected virtual string Get() => "";

            public TOut Get<TArg>(T arg1, TIn arg2, TArg arg3) where TArg : struct
            {
                return new TOut
                {
                    Value = $"{arg1.GetType().Name}_{arg2.GetType().Name}_{arg3.GetType().Name}{Get()}"
                };
            }
        }

        private class DerivedGenericService<T, TIn, TOut> : GenericService<T, TIn, TOut>
            where T : class, IBaseArgument, new()
            where TIn : struct
            where TOut : AnotherArgument, IBaseArgument, new()
        {
            private readonly string _value;

            protected override string Get() => $"_{_value}";

            public DerivedGenericService(string value)
            {
                _value = value;
            }
        }

        public class Argument
        {
            public string StringValue { get; }

            public Argument(string stringValue)
            {
                StringValue = stringValue;
            }
        }

        enum SomeEnum
        {
            Value1,
            Value2
        }

        public interface IServiceToTestOverrides
        {
            string Property { get; set; }
            string Get(string arg);
        }

        private class ServiceToTestOverrides : IServiceToTestOverrides
        {
            private readonly SomeEnum _someEnum;
            private readonly IService2 _service;
            private readonly Argument _argument;

            [Dependency]
            public string Property { get; set; }

            public string Get(string arg) => $"{_someEnum}_{_service.Method(arg)}_{Property}_{_argument.StringValue}";

            public ServiceToTestOverrides(SomeEnum someEnum, IService2 service, Argument argument)
            {
                _someEnum = someEnum;
                _service = service;
                _argument = argument;
            }
        }

        [Fact]
        public void ServiceCtorMustBeExecutedAfterMethodIsCalledAndOnlyOnce()
        {
            _service1Id = string.Empty;
            _service2Id = string.Empty;

            var service = new UnityContainer()
                .RegisterLazy<IService1, Service1>()
                .RegisterType<IService2, Service2>()
                .Resolve<IService1>();

            Assert.Empty(_service1Id);
            Assert.Empty(_service2Id);

            var result1 = service.MethodWithoutOtherServiceInvocation();

            Assert.Equal("service1", result1);
            Assert.NotEmpty(_service1Id);
            Assert.NotEmpty(_service2Id);

            var prevService1Id = _service1Id;
            var prevService2Id = _service2Id;

            var result2 = service.MethodWithoutOtherServiceInvocation();

            Assert.Equal("service1", result2);
            Assert.Equal(prevService1Id, _service1Id);
            Assert.Equal(prevService2Id, _service2Id);
        }

        [Fact]
        public void ServiceCtorMustBeExecutedAfterPropertyGetterIsCalled()
        {
            _service1Id = string.Empty;
            _service2Id = string.Empty;

            var service = new UnityContainer()
                .RegisterLazy<IService1, Service1>()
                .RegisterType<IService2, Service2>()
                .Resolve<IService1>();

            Assert.Empty(_service1Id);
            Assert.Empty(_service2Id);

            var result = service.Property;

            Assert.Equal("property", result);
            Assert.NotEmpty(_service1Id);
            Assert.NotEmpty(_service2Id);
        }

        [Fact]
        public void ServiceCtorMustBeExecutedAfterPropertySetterIsCalled()
        {
            _service1Id = string.Empty;
            _service2Id = string.Empty;

            var service = new UnityContainer()
                .RegisterLazy<IService1, Service1>()
                .RegisterType<IService2, Service2>()
                .Resolve<IService1>();

            Assert.Empty(_service1Id);
            Assert.Empty(_service2Id);

            service.Property = "newProperty";

            Assert.NotEmpty(_service1Id);
            Assert.NotEmpty(_service2Id);
        }

        [Fact]
        public void ServiceCtorMustBeExecutedAfterMethodIsCalledForAllNestedLazyTypes()
        {
            _service1Id = string.Empty;
            _service2Id = string.Empty;

            var service = new UnityContainer()
                .RegisterLazy<IService1, Service1>()
                .RegisterLazy<IService2, Service2>()
                .Resolve<IService1>();

            Assert.Empty(_service1Id);
            Assert.Empty(_service2Id);

            var result1 = service.MethodWithoutOtherServiceInvocation();

            Assert.Equal("service1", result1);
            Assert.NotEmpty(_service1Id);
            Assert.Empty(_service2Id);

            var result2 = service.MethodWithOtherServiceInvocation("test");
            Assert.Equal("service1->service2->test", result2);
            Assert.NotEmpty(_service1Id);
            Assert.NotEmpty(_service2Id);
        }

        [Fact]
        public void LifetimeMustBeCorrect()
        {
            _service1Id = string.Empty;
            _service2Id = string.Empty;

            var container = new UnityContainer()
                .RegisterLazy<IService1, Service1>(() => new ContainerControlledLifetimeManager())
                .RegisterLazy<IService2, Service2>(() => new ContainerControlledLifetimeManager());

            Assert.Empty(_service1Id);
            Assert.Empty(_service2Id);

            container.Resolve<IService1>().MethodWithOtherServiceInvocation("test1");

            Assert.NotEmpty(_service1Id);
            Assert.NotEmpty(_service2Id);

            var prevService1Id = _service1Id;
            var prevService2Id = _service2Id;

            container.Resolve<IService1>().MethodWithOtherServiceInvocation("test2");

            Assert.Equal(prevService1Id, _service1Id);
            Assert.Equal(prevService2Id, _service2Id);
        }

        [Fact]
        public void InjectionMembersMustBeCorrect()
        {
            const string arg = "test";
            const string result = "result";
            var service2Mock = new Mock<IService2>(MockBehavior.Strict);

            service2Mock.Setup(s => s.Method(arg)).Returns(result);

            var container = new UnityContainer()
                .RegisterLazy<IService1, Service1>(
                    new InjectionConstructor(service2Mock.Object));

            var actualResult = container.Resolve<IService1>().MethodWithOtherServiceInvocation(arg);

            Assert.Equal("service1->" + result, actualResult);
        }

        [Fact]
        public void ServicesMustBeResolvedFromChildContainer()
        {
            var container = new UnityContainer()
                .RegisterLazy<IService1, Service1>();

            Assert.Throws<ResolutionFailedException>(() =>
            {
                var service = container.Resolve<IService1>();
                service.MethodWithoutOtherServiceInvocation();
            });

            var childContainer = container.CreateChildContainer()
                .RegisterType<IService2, Service2>();

            var exception = Record.Exception(() =>
            {
                var service = childContainer.Resolve<IService1>();
                service.MethodWithoutOtherServiceInvocation();
            });

            Assert.Null(exception);
        }

        [Fact]
        public void ServicesMustBeResolvedByNameWithCorrectLifetime()
        {
            const string serviceName1 = "serviceName1";
            const string serviceName2 = "serviceName2";

            var container = new UnityContainer()
                .RegisterLazy<IService1, Service1>(serviceName1, () => new ContainerControlledLifetimeManager())
                .RegisterLazy<IService1, Service1>(serviceName2, () => new HierarchicalLifetimeManager());

            var childContainer = container.CreateChildContainer();

            Assert.Throws<ResolutionFailedException>(() => container.Resolve<IService1>());
            Assert.Same(container.Resolve<IService1>(serviceName1), container.Resolve<IService1>(serviceName1));
            Assert.Same(container.Resolve<IService1>(serviceName2), container.Resolve<IService1>(serviceName2));
            Assert.NotSame(container.Resolve<IService1>(serviceName1), container.Resolve<IService1>(serviceName2));

            Assert.Throws<ResolutionFailedException>(() => childContainer.Resolve<IService1>());
            Assert.Same(childContainer.Resolve<IService1>(serviceName1), childContainer.Resolve<IService1>(serviceName1));
            Assert.Same(childContainer.Resolve<IService1>(serviceName2), childContainer.Resolve<IService1>(serviceName2));

            Assert.NotSame(childContainer.Resolve<IService1>(serviceName1), childContainer.Resolve<IService1>(serviceName2));
            Assert.Same(container.Resolve<IService1>(serviceName1), childContainer.Resolve<IService1>(serviceName1));
            Assert.NotSame(container.Resolve<IService1>(serviceName2), childContainer.Resolve<IService1>(serviceName2));
        }

        [Fact]
        public void ServicesMustBeResolvedByNameWithCorrectInjectionMembers()
        {
            const string arg = "arg";
            const string result1 = "result1";
            const string result2 = "result2";
            const string serviceName1 = "serviceName1";
            const string serviceName2 = "serviceName2";

            var serviceMock1 = new Mock<IService2>(MockBehavior.Strict);
            serviceMock1.Setup(s => s.Method(arg)).Returns(result1);

            var serviceMock2 = new Mock<IService2>(MockBehavior.Strict);
            serviceMock2.Setup(s => s.Method(arg)).Returns(result2);

            var container = new UnityContainer()
                .RegisterLazy<IService1, Service1>(serviceName1, new InjectionConstructor(serviceMock1.Object))
                .RegisterLazy<IService1, Service1>(serviceName2, new InjectionConstructor(serviceMock2.Object));

            var actualResult1 = container.Resolve<IService1>(serviceName1).MethodWithOtherServiceInvocation(arg);
            var actualResult2 = container.Resolve<IService1>(serviceName2).MethodWithOtherServiceInvocation(arg);

            Assert.Throws<ResolutionFailedException>(() => container.Resolve<IService1>());
            Assert.Equal("service1->" + result1, actualResult1);
            Assert.Equal("service1->" + result2, actualResult2);
        }

        [Fact]
        public void RegistrationMustThrowAnExceptionForNonInterfaces()
        {
            Assert.Throws<NotSupportedException>(() => new UnityContainer().RegisterLazy<Service1, Service1>());
        }

        [Fact]
        public void InternalsVisibleToAttributeMustAllowToResolveInternalServices()
        {
            var result = new UnityContainer()
                .RegisterLazy<IInternalService, InternalService>()
                .Resolve<IInternalService>()
                .Get();

            Assert.Equal("InternalService", result);
        }

        [Fact]
        public void ClosedGenericServiceMustBeResolved()
        {
            var result = new UnityContainer()
                .RegisterLazy(
                    typeof(IGenericService<Argument1A, Argument2, Argument3>),
                    typeof(GenericService<Argument1A, Argument2, Argument3>))
                .Resolve<IGenericService<Argument1A, Argument2, Argument3>>()
                .Get(new Argument1A(), new Argument2(), 42);

            Assert.Equal("Argument1A_Argument2_Int32", result.Value);
        }

        [Fact]
        public void OpenedGenericServiceMustBeResolved()
        {
            var result = new UnityContainer()
                .RegisterLazy(typeof(IGenericService<,,>), typeof(GenericService<,,>))
                .Resolve<IGenericService<Argument1A, Argument2, Argument3>>()
                .Get(new Argument1A(), new Argument2(), 42);

            Assert.Equal("Argument1A_Argument2_Int32", result.Value);
        }

        [Fact]
        public void GenericServiceMustBeResolvedWithCorrectLifetime()
        {
            var container = new UnityContainer()
                .RegisterLazy(
                    typeof(IGenericService<,,>),
                    typeof(GenericService<,,>),
                    () => new TransientLifetimeManager())
                .RegisterLazy(
                    typeof(IGenericService<Argument1A, Argument2, Argument3>),
                    typeof(GenericService<Argument1A, Argument2, Argument3>),
                    () => new ContainerControlledLifetimeManager())
                .RegisterLazy(
                    typeof(IGenericService<Argument1B, Argument2, Argument3>),
                    typeof(GenericService<Argument1B, Argument2, Argument3>),
                    () => new HierarchicalLifetimeManager())
                .RegisterLazy(
                    typeof(IGenericService<Argument1C, Argument2, Argument3>),
                    typeof(GenericService<Argument1C, Argument2, Argument3>),
                    () => new TransientLifetimeManager());

            var service1 = container.Resolve<IGenericService<Argument1A, Argument2, Argument3>>();
            var service2 = container.Resolve<IGenericService<Argument1B, Argument2, Argument3>>();
            var service3 = container.Resolve<IGenericService<Argument1C, Argument2, Argument3>>();

            Assert.Same(service1, container.Resolve<IGenericService<Argument1A, Argument2, Argument3>>());
            Assert.Same(service2, container.Resolve<IGenericService<Argument1B, Argument2, Argument3>>());
            Assert.NotSame(service3, container.Resolve<IGenericService<Argument1C, Argument2, Argument3>>());

            using (var childContainer = container.CreateChildContainer())
            {
                Assert.Same(service1, childContainer.Resolve<IGenericService<Argument1A, Argument2, Argument3>>());
                Assert.NotSame(service2, childContainer.Resolve<IGenericService<Argument1B, Argument2, Argument3>>());
                Assert.NotSame(service3, childContainer.Resolve<IGenericService<Argument1C, Argument2, Argument3>>());
            }
        }

        [Fact]
        public void GenericServicesMustBeResolvedByNameWithCorrectLifetime()
        {
            const string singleton = "singleton";
            const string hierarchical = "hierarchical";
            const string transient = "transient";

            var container = new UnityContainer()
                .RegisterLazy(
                    typeof(IGenericService<,,>),
                    typeof(GenericService<,,>),
                    singleton,
                    () => new ContainerControlledLifetimeManager())
                .RegisterLazy(
                    typeof(IGenericService<,,>),
                    typeof(GenericService<,,>),
                    hierarchical,
                    () => new HierarchicalLifetimeManager())
                .RegisterLazy(
                    typeof(IGenericService<,,>),
                    typeof(GenericService<,,>),
                    transient,
                    () => new TransientLifetimeManager());

            var service1 = container.Resolve<IGenericService<Argument1A, Argument2, Argument3>>(singleton);
            var service2 = container.Resolve<IGenericService<Argument1B, Argument2, Argument3>>(hierarchical);
            var service3 = container.Resolve<IGenericService<Argument1C, Argument2, Argument3>>(transient);

            Assert.Same(service1, container.Resolve<IGenericService<Argument1A, Argument2, Argument3>>(singleton));
            Assert.Same(service2, container.Resolve<IGenericService<Argument1B, Argument2, Argument3>>(hierarchical));
            Assert.NotSame(service3, container.Resolve<IGenericService<Argument1C, Argument2, Argument3>>(transient));

            using (var childContainer = container.CreateChildContainer())
            {
                Assert.Same(service1, childContainer.Resolve<IGenericService<Argument1A, Argument2, Argument3>>(singleton));
                Assert.NotSame(service2, childContainer.Resolve<IGenericService<Argument1B, Argument2, Argument3>>(hierarchical));
                Assert.NotSame(service3, childContainer.Resolve<IGenericService<Argument1C, Argument2, Argument3>>(transient));
            }
        }

        [Fact]
        public void GenericServicesMustBeResolvedByNameWithCorrectInjectionMembers()
        {
            const string value1 = "value1";
            const string value2 = "value2";

            var container = new UnityContainer()
                .RegisterLazy(
                    typeof(IGenericService<,,>),
                    typeof(DerivedGenericService<,,>),
                    value1,
                    new InjectionConstructor(value1))
                .RegisterLazy(
                    typeof(IGenericService<,,>),
                    typeof(DerivedGenericService<,,>),
                    value2,
                    new InjectionConstructor(value2));

            var service1 = container.Resolve<IGenericService<Argument1A, Argument2, Argument3>>(value1);
            var service2 = container.Resolve<IGenericService<Argument1A, Argument2, Argument3>>(value2);

            Assert.Equal($"Argument1A_Argument2_Int32_{value1}", service1.Get(new Argument1A(), new Argument2(), 42).Value);
            Assert.Equal($"Argument1A_Argument2_Int32_{value2}", service2.Get(new Argument1A(), new Argument2(), 42).Value);
        }

        [Fact]
        public void OverridesMustBeAppliedByProxy()
        {
            var result = new UnityContainer()
                .RegisterType<IService2, Service2>()
                .RegisterLazy<IServiceToTestOverrides, ServiceToTestOverrides>()
                .Resolve<IServiceToTestOverrides>(
                    new ParameterOverride("someEnum", SomeEnum.Value2),
                    new DependencyOverride(typeof(IService2), new Service2Ex()),
                    new PropertyOverride("Property", "propertyValue"),
                    new TypeBasedOverride(typeof(Argument), new ParameterOverride("stringValue", "stringValue"))
                )
                .Get("arg");

            Assert.Equal("Value2_service2Ex->arg_propertyValue_stringValue", result);
        }
    }
}