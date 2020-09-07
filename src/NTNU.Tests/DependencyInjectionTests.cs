using System;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using LightInject;

namespace NTNU.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void WithoutDependencyInjection()
        {
            var orderService = new OrderServiceWithoutDependencyInjection();
            orderService.Save(new Order());
        }

        [Fact]
        public void WithDependencyInjection()
        {
            var orderService = new OrderServiceWithDependencyInjection(new EMailOrderNotifier());
            orderService.Save(new Order());
        }


        [Fact]
        public void ShouldNotifyWhenSaved()
        {
            var orderNotifierMock = new OrderNotifierMock();
            var orderService = new OrderServiceWithDependencyInjection(orderNotifierMock);

            orderService.Save(new Order());

            orderNotifierMock.WasCalled.Should().BeTrue();
        }

        [Fact]
        public void WithServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IOrderNotifier, EMailOrderNotifier>();
            serviceCollection.AddTransient<IOrderService, OrderServiceWithDependencyInjection>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var orderService = serviceProvider.GetService<IOrderService>();

            orderService.Save(new Order());
        }


        [Fact]
        public void WithServiceContainer()
        {
            var container = new ServiceContainer();
            container.Register<IOrderNotifier, EMailOrderNotifier>();
            container.Register<IOrderService, OrderServiceWithDependencyInjection>();

            var orderService = container.GetInstance<IOrderService>();
            orderService.Save(new Order());
        }


        [Fact]
        public void WithManualDecorator()
        {
            var orderService = new OrderServiceWithDependencyInjection(new LoggedOrderNotifierDecorator(new EMailOrderNotifier()));
            orderService.Save(new Order());
        }


        [Fact]
        public void DecoratorsUsingServiceContainer()
        {
            var container = new ServiceContainer();
            container.Register<IOrderNotifier, EMailOrderNotifier>();
            container.Register<IOrderService, OrderServiceWithDependencyInjection>();

            container.Decorate<IOrderNotifier, LoggedOrderNotifierDecorator>();

            var orderService = container.GetInstance<IOrderService>();
            orderService.Save(new Order());
        }


        [Fact]
        public void Transient()
        {
            var container = new ServiceContainer();
            container.RegisterTransient<IOrderService, OrderServiceWithDependencyInjection>();
            container.RegisterTransient<IOrderNotifier, EMailOrderNotifier>();

            var firstOrderService = container.GetInstance<IOrderService>();
            var secondOrderService = container.GetInstance<IOrderService>();

            firstOrderService.Should().NotBeSameAs(secondOrderService);
        }

        [Fact]
        public void Singleton()
        {
            var container = new ServiceContainer();
            container.RegisterSingleton<IOrderService, OrderServiceWithDependencyInjection>();
            container.RegisterSingleton<IOrderNotifier, EMailOrderNotifier>();

            var firstOrderService = container.GetInstance<IOrderService>();
            var secondOrderService = container.GetInstance<IOrderService>();

            firstOrderService.Should().BeSameAs(secondOrderService);
        }



        [Fact]
        public void UsingWithDisposable()
        {
            using (var notifier = new EMailOrderNotifier())
            {

            }// << Dispose is called here
        }


        [Fact(Skip = "Failing")]
        public void Scoped_Failing()
        {
            var container = new ServiceContainer();
            container.RegisterScoped<IOrderService, OrderServiceWithDependencyInjection>();
            container.RegisterScoped<IOrderNotifier, EMailOrderNotifier>();

            var firstOrderService = container.GetInstance<IOrderService>();
            var secondOrderService = container.GetInstance<IOrderService>();

            firstOrderService.Should().BeSameAs(secondOrderService);
        }

        [Fact]
        public void Scoped_Passing()
        {
            var container = new ServiceContainer();
            container.RegisterScoped<IOrderService, OrderServiceWithDependencyInjection>();
            container.RegisterScoped<IOrderNotifier, EMailOrderNotifier>();

            using (var scope = container.BeginScope())
            {
                var firstOrderService = scope.GetInstance<IOrderService>();
                var secondOrderService = scope.GetInstance<IOrderService>();
                firstOrderService.Should().BeSameAs(secondOrderService);
            }
        }

        public void Captive()
        {
            // Don't do this

            var container = new ServiceContainer();
            container.RegisterTransient<IOrderNotifier, EMailOrderNotifier>();
            container.RegisterSingleton<IOrderService, OrderServiceWithDependencyInjection>();

            var orderService = container.GetInstance<IOrderService>();

            // Problem is that EMailOrderNotifier is now effectively a singleton since
            // it gets injected into a singleton (OrderServiceWithDependencyInjection)
        }



    }


    public class Order
    {
    }

    public interface IOrderService
    {
        void Save(Order order);
    }

    public interface IOrderNotifier
    {
        void Notify(Order order);
    }

    public class EMailOrderNotifier : IOrderNotifier, IDisposable
    {
        public void Dispose()
        {
            // Nothing to do here
        }

        public void Notify(Order order)
        {
            Console.WriteLine("Notify customer via email");
        }
    }

    public class OrderServiceWithoutDependencyInjection : IOrderService
    {
        private IOrderNotifier orderNotifier = new EMailOrderNotifier();

        public OrderServiceWithoutDependencyInjection()
        {
        }

        public void Save(Order order)
        {
            Console.WriteLine("Saving order");
            orderNotifier.Notify(order);
        }
    }

    public class OrderServiceWithDependencyInjection : IOrderService
    {
        private readonly IOrderNotifier orderNotifier;

        public OrderServiceWithDependencyInjection(IOrderNotifier orderNotifier)
        {
            this.orderNotifier = orderNotifier;
        }

        public void Save(Order order)
        {
            Console.WriteLine("Saving order");
            orderNotifier.Notify(order);
        }
    }


    public class OrderNotifierMock : IOrderNotifier
    {
        public void Notify(Order order)
        {
            WasCalled = true;
        }

        public bool WasCalled { get; set; }
    }

    public class LoggedOrderNotifierDecorator : IOrderNotifier
    {
        private readonly IOrderNotifier orderNotifier;

        public LoggedOrderNotifierDecorator(IOrderNotifier orderNotifier)
        {
            this.orderNotifier = orderNotifier;
        }

        public void Notify(Order order)
        {
            orderNotifier.Notify(order);
            Console.WriteLine("Log message : Customer was notified");
        }
    }
}