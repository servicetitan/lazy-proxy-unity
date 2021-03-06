﻿using System;
using Unity;

namespace LazyProxy.Unity.Sample
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("--- UnityExtension example #1 ---");
            UnityExtensionExample1();

            Console.WriteLine("--- UnityExtension example #2 ---");
            UnityExtensionExample2();
        }

        private static void UnityExtensionExample1()
        {
            // Creating a container
            using var container = new UnityContainer();

            // Adding a lazy registration
            container.RegisterLazy<IMyService, MyService>();

            Console.WriteLine("Resolving the service...");
            var service = container.Resolve<IMyService>();

            Console.WriteLine("Executing the 'Foo' method...");
            service.Foo();
        }

        private static void UnityExtensionExample2()
        {
            var container = new UnityContainer()
                .RegisterLazy<IWeaponService, WeaponService>()
                .RegisterLazy<INinjaService, NinjaService>();

            Console.WriteLine("Resolving INinjaService...");
            var service = container.Resolve<INinjaService>();
            Console.WriteLine($"Type of 'ninjaService' is {service.GetType()}");

            Console.WriteLine("Invoking property getter 'service.MinNinjaHealth' ...");
            var minDamage = service.MinNinjaHealth;

            Console.WriteLine("Invoking method 'service.CreateNinja'...");
            var ninja = service.CreateNinja();

            Console.WriteLine("Invoking parent method 'service.GetDamage'...");
            var damage = service.GetDamage(ninja);

            try
            {
                Console.WriteLine("Invoking parent method 'service.GetDamage' with not correct argument...");
                service.GetDamage(null);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine($"Expected exception is thrown: {e.Message}");
            }
        }
    }
}
