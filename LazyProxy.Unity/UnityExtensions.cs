using System;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Registration;

namespace LazyProxy.Unity
{
    public static class UnityExtensions
    {
        private static readonly Func<LifetimeManager> GetTransientLifetimeManager = () => new TransientLifetimeManager();

        /// <summary>
        /// Is used to register interface TFrom to class TTo by creation a lazy proxy at runtime.
        /// The real class To will be instantiated only after first method execution.
        /// </summary>
        /// <param name="container">The instance of Unity container.</param>
        /// <param name="injectionMembers">The set of injection members.</param>
        /// <typeparam name="TFrom">The binded interface.</typeparam>
        /// <typeparam name="TTo">The binded class.</typeparam>
        /// <returns>The instance of Unity container.</returns>
        public static IUnityContainer RegisterLazy<TFrom, TTo>(this IUnityContainer container,
            params InjectionMember[] injectionMembers)
            where TTo : TFrom where TFrom : class =>
            container.RegisterLazy<TFrom, TTo>(null, GetTransientLifetimeManager, injectionMembers);

        /// <summary>
        /// Is used to register interface TFrom to class TTo by creation a lazy proxy at runtime.
        /// The real class To will be instantiated only after first method or property execution.
        /// </summary>
        /// <param name="container">The instance of Unity container.</param>
        /// <param name="name">The registration name.</param>
        /// <param name="injectionMembers">The set of injection members.</param>
        /// <typeparam name="TFrom">The binded interface.</typeparam>
        /// <typeparam name="TTo">The binded class.</typeparam>
        /// <returns>The instance of Unity container.</returns>
        public static IUnityContainer RegisterLazy<TFrom, TTo>(this IUnityContainer container,
            string name,
            params InjectionMember[] injectionMembers)
            where TTo : TFrom where TFrom : class =>
            container.RegisterLazy<TFrom, TTo>(name, GetTransientLifetimeManager, injectionMembers);

        /// <summary>
        /// Is used to register interface TFrom to class TTo by creation a lazy proxy at runtime.
        /// The real class To will be instantiated only after first method or property execution.
        /// </summary>
        /// <param name="container">The instance of Unity container.</param>
        /// <param name="getLifetimeManager">The function instance lifetime provides.</param>
        /// <param name="injectionMembers">The set of injection members.</param>
        /// <typeparam name="TFrom">The binded interface.</typeparam>
        /// <typeparam name="TTo">The binded class.</typeparam>
        /// <returns>The instance of Unity container.</returns>
        public static IUnityContainer RegisterLazy<TFrom, TTo>(this IUnityContainer container,
            Func<LifetimeManager> getLifetimeManager,
            params InjectionMember[] injectionMembers)
            where TTo : TFrom where TFrom : class =>
            container.RegisterLazy<TFrom, TTo>(null, getLifetimeManager, injectionMembers);

        /// <summary>
        /// Is used to register interface TFrom to class TTo by creation a lazy proxy at runtime.
        /// The real class To will be instantiated only after first method or property execution.
        /// </summary>
        /// <param name="container">The instance of Unity container.</param>
        /// <param name="name">The registration name.</param>
        /// <param name="getLifetimeManager">The function instance lifetime provides.</param>
        /// <param name="injectionMembers">The set of injection members.</param>
        /// <typeparam name="TFrom">The binded interface.</typeparam>
        /// <typeparam name="TTo">The binded class.</typeparam>
        /// <returns>The instance of Unity container.</returns>
        public static IUnityContainer RegisterLazy<TFrom, TTo>(this IUnityContainer container,
            string name,
            Func<LifetimeManager> getLifetimeManager,
            params InjectionMember[] injectionMembers)
            where TTo : TFrom where TFrom : class
        {
            // There is no way to constraint it on the compilation step.
            if (!typeof(TFrom).IsInterface)
            {
                throw new NotSupportedException("The lazy registration is supported only for interfaces.");
            }

            var lazyProxyType = LazyProxyBuilder.BuildLazyProxyType<TFrom>();
            var registrationName = Guid.NewGuid().ToString();

            return container
                .RegisterType<TFrom, TTo>(registrationName, getLifetimeManager(), injectionMembers)
                .RegisterType(typeof(TFrom), lazyProxyType, name,
                    getLifetimeManager(),
                    new InjectionConstructor(
                        new ResolvedParameter<Lazy<TFrom>>(registrationName))
                );
        }
    }
}