using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;

namespace LazyProxy.Unity
{
    /// <summary>
    /// A class that lets you specify a factory method the container will use to create the object.
    /// </summary>
    public class UnityInjectionFactory : InjectionMember, IInjectionFactory, IBuildPlanPolicy
    {
        private static readonly FieldInfo ResolverOverrides = typeof(BuilderContext)
            .GetField("_resolverOverrides", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly Func<IUnityContainer, Type, string, ResolverOverride[], object> _factoryFunc;

        /// <summary>
        /// Create a new instance of <see cref="UnityInjectionFactory"/> with the given factory function.
        /// </summary>
        /// <param name="factoryFunc">Factory function.</param>
        public UnityInjectionFactory(Func<IUnityContainer, Type, string, ResolverOverride[], object> factoryFunc)
        {
            _factoryFunc = factoryFunc ?? throw new ArgumentNullException(nameof(factoryFunc));
        }

        /// <summary>
        /// Add policies to the policies to configure the container
        /// to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="serviceType">Type of interface being registered. If no interface, this will be null.
        /// This parameter is ignored in this implementation.</param>
        /// <param name="implementationType">Type of concrete type being registered.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            policies.Set(serviceType, name, typeof(IBuildPlanPolicy), this);
        }

        /// <summary>
        /// Creates an instance of this build plan's type, or fills in the existing type if passed in.
        /// </summary>
        /// <param name="context">Context used to build up the object.</param>
        /// <exception cref="ArgumentNullException">Context is null.</exception>
        public void BuildUp(IBuilderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Existing != null)
                return;

            var resolverOverride = ResolverOverrides.GetValue(context);

            var resolverOverrides = resolverOverride == null
                ? new ResolverOverride[] { }
                : ((IEnumerable<ResolverOverride>)resolverOverride).ToArray();

            var container = context.Container;
            var type = context.BuildKey.Type;
            var name = context.BuildKey.Name;

            context.Existing = _factoryFunc(container, type, name, resolverOverrides);
            context.SetPerBuildSingleton();
        }
    }
}