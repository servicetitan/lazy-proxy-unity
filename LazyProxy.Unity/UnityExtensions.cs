using System;
using System.Linq.Expressions;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Registration;

namespace LazyProxy.Unity
{
    /// <summary>
    /// Extension methods for lazy registration.
    /// </summary>
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
            where TTo : TFrom where TFrom : class =>
            container.RegisterLazy(typeof(TFrom), typeof(TTo), name, getLifetimeManager, injectionMembers);

        /// <summary>
        /// Is used to register interface TFrom to class TTo by creation a lazy proxy at runtime.
        /// The real class To will be instantiated only after first method execution.
        /// </summary>
        /// <param name="typeFrom">The binded interface.</param>
        /// <param name="typeTo">The binded class.</param>
        /// <param name="container">The instance of Unity container.</param>
        /// <param name="injectionMembers">The set of injection members.</param>
        /// <returns>The instance of Unity container.</returns>
        public static IUnityContainer RegisterLazy(this IUnityContainer container,
            Type typeFrom,
            Type typeTo,
            params InjectionMember[] injectionMembers) =>
            container.RegisterLazy(typeFrom, typeTo, null, GetTransientLifetimeManager, injectionMembers);

        /// <summary>
        /// Is used to register interface TFrom to class TTo by creation a lazy proxy at runtime.
        /// The real class To will be instantiated only after first method or property execution.
        /// </summary>
        /// <param name="typeFrom">The binded interface.</param>
        /// <param name="typeTo">The binded class.</param>
        /// <param name="container">The instance of Unity container.</param>
        /// <param name="name">The registration name.</param>
        /// <param name="injectionMembers">The set of injection members.</param>
        /// <returns>The instance of Unity container.</returns>
        public static IUnityContainer RegisterLazy(this IUnityContainer container,
            Type typeFrom,
            Type typeTo,
            string name,
            params InjectionMember[] injectionMembers) =>
            container.RegisterLazy(typeFrom, typeTo, name, GetTransientLifetimeManager, injectionMembers);

        /// <summary>
        /// Is used to register interface TFrom to class TTo by creation a lazy proxy at runtime.
        /// The real class To will be instantiated only after first method or property execution.
        /// </summary>
        /// <param name="typeFrom">The binded interface.</param>
        /// <param name="typeTo">The binded class.</param>
        /// <param name="container">The instance of Unity container.</param>
        /// <param name="getLifetimeManager">The function instance lifetime provides.</param>
        /// <param name="injectionMembers">The set of injection members.</param>
        /// <returns>The instance of Unity container.</returns>
        public static IUnityContainer RegisterLazy(this IUnityContainer container,
            Type typeFrom,
            Type typeTo,
            Func<LifetimeManager> getLifetimeManager,
            params InjectionMember[] injectionMembers) =>
            container.RegisterLazy(typeFrom, typeTo, null, getLifetimeManager, injectionMembers);

        /// <summary>
        /// Is used to register interface TFrom to class TTo by creation a lazy proxy at runtime.
        /// The real class To will be instantiated only after first method or property execution.
        /// </summary>
        /// <param name="typeFrom">The binded interface.</param>
        /// <param name="typeTo">The binded class.</param>
        /// <param name="container">The instance of Unity container.</param>
        /// <param name="name">The registration name.</param>
        /// <param name="getLifetimeManager">The function instance lifetime provides.</param>
        /// <param name="injectionMembers">The set of injection members.</param>
        /// <returns>The instance of Unity container.</returns>
        public static IUnityContainer RegisterLazy(this IUnityContainer container,
            Type typeFrom,
            Type typeTo,
            string name,
            Func<LifetimeManager> getLifetimeManager,
            params InjectionMember[] injectionMembers)
        {
            // There is no way to constraint it on the compilation step.
            if (!typeFrom.IsInterface)
            {
                throw new NotSupportedException("The lazy registration is supported only for interfaces.");
            }

            var lazyProxyType = LazyProxyBuilder.BuildLazyProxyType(typeFrom);
            var registrationName = Guid.NewGuid().ToString();

            return container
                .RegisterType(typeFrom, typeTo, registrationName, getLifetimeManager(), injectionMembers)
                .RegisterType(typeFrom, name, getLifetimeManager(), new InjectionFactory(
                    (c, t, n) =>
                    {
                        var valueFactory = BuildValueFactory(c, t, registrationName);
                        var lazy = Activator.CreateInstance(typeof(Lazy<>).MakeGenericType(t), valueFactory);

                        var closedLazyProxyType = lazyProxyType.IsGenericTypeDefinition
                            ? lazyProxyType.MakeGenericType(t.GenericTypeArguments)
                            : lazyProxyType;

                        return Activator.CreateInstance(closedLazyProxyType, lazy);
                    }));
        }

        private static Delegate BuildValueFactory(IUnityContainer container, Type type, string name)
        {
            Expression<Func<object>> expression = () => container.Resolve(type, name);
            var body = Expression.Convert(expression.Body, type);
            var lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(type), body);
            return lambda.Compile();
        }
    }
}