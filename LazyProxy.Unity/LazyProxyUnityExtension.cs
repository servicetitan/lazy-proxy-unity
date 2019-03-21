using System;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;

namespace LazyProxy.Unity
{
    /// <summary>
    /// Allows to add a lazy registration to the container.
    /// </summary>
    public class LazyProxyUnityExtension : UnityContainerExtension
    {
        private readonly Type _typeFrom;
        private readonly Type _typeTo;
        private readonly string _name;
        private readonly Func<ITypeLifetimeManager> _getLifetimeManager;
        private readonly InjectionMember[] _injectionMembers;

        /// <summary>
        /// Initialize a new instance of <see cref="LazyProxyUnityExtension"/>.
        /// </summary>
        /// <param name="typeFrom">The binded interface.</param>
        /// <param name="typeTo">The binded class.</param>
        /// <param name="name">The registration name.</param>
        /// <param name="getLifetimeManager">The function instance lifetime provides.</param>
        /// <param name="injectionMembers">The set of injection members.</param>
        public LazyProxyUnityExtension(
            Type typeFrom,
            Type typeTo,
            string name,
            Func<ITypeLifetimeManager> getLifetimeManager,
            params InjectionMember[] injectionMembers)
        {
            _typeFrom = typeFrom ?? throw new ArgumentNullException(nameof(typeFrom));
            _typeTo = typeTo ?? throw new ArgumentNullException(nameof(typeTo));;
            _name = name;
            _getLifetimeManager = getLifetimeManager ?? throw new ArgumentNullException(nameof(getLifetimeManager));;
            _injectionMembers = injectionMembers;
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            var registrationName = Guid.NewGuid().ToString();

            Context.Container.RegisterType(_typeFrom, _typeTo, _name, _getLifetimeManager());
            Context.Container.RegisterType(_typeFrom, _typeTo, registrationName, _getLifetimeManager(), _injectionMembers);

            Context.Policies.Set(_typeFrom, _name, typeof(ResolveDelegateFactory),
                (ResolveDelegateFactory) ((ref BuilderContext _) =>
                    (ref BuilderContext c) =>
                    {
                        var container = c.Container;
                        var type = c.RegistrationType;
                        var overrides = c.Overrides;

                        return LazyProxyBuilder.CreateInstance(type,
                            () => container.Resolve(type, registrationName, overrides)
                        );
                    })
            );
        }
    }
}