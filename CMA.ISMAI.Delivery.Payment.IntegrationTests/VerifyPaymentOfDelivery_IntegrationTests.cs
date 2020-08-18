using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Core.Service;
using CMA.ISMAI.Delivery.EventStore.Interface;
using CMA.ISMAI.Delivery.EventStore.Service;
using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.Logging.Service;
using CMA.ISMAI.Delivery.Payment.CrossCutting.Bus;
using CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda.Interface;
using CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda.Service;
using CMA.ISMAI.Delivery.Payment.CrossCutting.FileReader;
using CMA.ISMAI.Delivery.Payment.CrossCutting.Queue;
using CMA.ISMAI.Delivery.Payment.Domain.Commands;
using CMA.ISMAI.Delivery.Payment.Domain.Events;
using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using CMA.ISMAI.Delivery.Payment.Domain.Model;
using CMA.ISMAI.Delivery.Payment.Domain.Model.Events;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Mediator;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace CMA.ISMAI.Delivery.FileLoading.IntegrationTests
{
    public class VerifyPaymentOfDelivery_IntegrationTests
    {
        [Fact(DisplayName = "Valid Payment Command")]
        [Trait("VerifyPaymentOfDeliveryCommand", "Payment has been done")]
        public async Task PaymentDone()
        {
            // Arrange
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            var deliveryFile = new VerifyPaymentOfDeliveryCommand("A029216", "ISMAI", "Informática", "C:\\Users\\Carlos Campos\\Desktop\\excel.xlsx");
            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(deliveryFile);
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Dind't find payment Command")]
        [Trait("VerifyPaymentOfDeliveryCommand", "Payment has been done")]
        public async Task PaymentNotFound()
        {
            // Arrange
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            var deliveryFile = new VerifyPaymentOfDeliveryCommand("A03943", "ISMAI", "Informática", "C:\\Users\\Carlos Campos\\Desktop\\excel.xlsx");
            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(deliveryFile);
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "File not found Command")]
        [Trait("VerifyPaymentOfDeliveryCommand", "Payment has been done")]
        public async Task FileNotFound()
        {
            // Arrange
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            var deliveryFile = new VerifyPaymentOfDeliveryCommand("A029216", "ISMAI", "Informática", "C:\\Users\\Carlos Campos\\Desktop\\notfound.xlsx");
            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(deliveryFile);
            Assert.False(result.IsValid);
        }


        private static IServiceCollection ConfigureServices()
        {

            IServiceCollection services = new ServiceCollection();
            var _config = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .Build();
            Log.Logger = new LoggerConfiguration()
           .Enrich.FromLogContext()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(_config.GetSection("ElasticConfiguration:Uri").Value))
            {
                AutoRegisterTemplate = true,
            })
        .CreateLogger();

            services.AddLogging();
            services.AddScoped<ILoggingService, LoggingService>();

            services.AddScoped<IMediatorHandler, InMemoryBus>();
            services.AddScoped<IEventStoreService, EventStoreService>();
            services.AddScoped<IRequestHandler<VerifyPaymentOfDeliveryCommand, ValidationResult>, DeliveryPaymentHandler>();
            services.AddScoped<INotificationHandler<PaymentCompletedEvent>, PaymentEventHandler>();
            services.AddScoped<IFileReaderService, FileReaderService>();
            services.AddScoped<ICamundaService, CamundaService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IQueueService, QueueService>();
            services.AddMediatR(typeof(VerifyPaymentOfDelivery_IntegrationTests).GetTypeInfo().Assembly);

            return services;
        }
    }
}
