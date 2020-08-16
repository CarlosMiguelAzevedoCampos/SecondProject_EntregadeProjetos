using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Core.Service;
using CMA.ISMAI.Delivery.Payment.CrossCutting.Bus;
using CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda.Interface;
using CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda.Service;
using CMA.ISMAI.Delivery.Payment.CrossCutting.FileReader;
using CMA.ISMAI.Delivery.Payment.CrossCutting.Queue;
using CMA.ISMAI.Delivery.Payment.Domain.Commands;
using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using CMA.ISMAI.Delivery.Payment.Domain.Model;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Mediator;
using System;
using System.Reflection;

namespace CMA.ISMAI.Delivery.Payment.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            Console.WriteLine(string.Format("Payment is starting..! - {0}", DateTime.Now));
            serviceProvider.GetRequiredService<ConsoleApplication>().StartService();
            Console.ReadKey();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<IMediatorHandler, InMemoryBus>();
            services.AddScoped<IRequestHandler<NotifyActionCommand, ValidationResult>, NotifyActionCommandHandler>();
            services.AddScoped<IRequestHandler<VerifyPaymentOfDeliveryCommand, ValidationResult>, DeliveryPaymentHandler>();
            services.AddScoped<IFileReaderService, FileReaderService>();
            services.AddScoped<ICamundaService, CamundaService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IQueueService, QueueService>();
            services.AddMediatR(typeof(Program).GetTypeInfo().Assembly);
            services.AddTransient<ConsoleApplication>();
            return services;
        }
    }
}
