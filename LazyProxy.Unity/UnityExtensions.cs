using System;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace LazyProxy.Unity
{
    /// <summary>
    /// Extension methods for lazy registration.
    /// </summary>
    public static class UnityExtensions
    {
        private static readonly Func<ITypeLifetimeManager> GetTransientLifetimeManager =
            () => new TransientLifetimeManager();

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
            Func<ITypeLifetimeManager> getLifetimeManager,
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
            Func<ITypeLifetimeManager> getLifetimeManager,
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
            Func<ITypeLifetimeManager> getLifetimeManager,
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
            Func<ITypeLifetimeManager> getLifetimeManager,
            params InjectionMember[] injectionMembers)
        {
            // There is no way to constraint it on the compilation step.
            if (!typeFrom.IsInterface)
            {
                throw new NotSupportedException("The lazy registration is supported only for interfaces.");
            }

//          Note #1
//          -------
//          If UnityContainer has the correct overload of the '.RegisterFactory' method we can make it easier:
//
//          var registrationName = Guid.NewGuid().ToString();
//          return container
//              .RegisterType(typeFrom, typeTo, registrationName, getLifetimeManager(), injectionMembers)
//              .RegisterFactory(typeFrom, name,
//                  (c, t, n, o) => LazyProxyBuilder.CreateInstance(t, () => c.Resolve(t, registrationName, o)),
//                  getLifetimeManager());
//
//          We opened an issue on GitHub and suggested pull requests to introduce the overload:
//              - https://github.com/unitycontainer/container/issues/147
//              - https://github.com/unitycontainer/abstractions/pull/98
//              - https://github.com/unitycontainer/container/pull/148
//
//          But unfortunately the issue and pull requests were rejected for reasons not entirely clear.
//          That is why we have to use extension.
//
//          Note #2
//          -------
//          We have to use an extension per type because UnityContainer has weird behaviour when work with
//          open generic types therefore we can't use fake interface to avoid multiple extensions:
//
//          public interface ILazyProxy<T> {}
//
//          ...then register types like this:
//          container.Register(_typeFrom, _typeTo,  "LazyProxyImpl" + _name);
//          container.Register(_typeFrom, typeof(ILazyProxy<>).MakeGenericType(_typeFrom), _name);
//
//          ...and use single extension per container:
//          context.Policy.Set(typeof(ILazyProxy<>), ...)

            return container.AddExtension(
                new LazyProxyUnityExtension(typeFrom, typeTo, name, getLifetimeManager, injectionMembers)
            );
        }
    }
}