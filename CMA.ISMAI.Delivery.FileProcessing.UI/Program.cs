using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Core.Service;
using CMA.ISMAI.Delivery.EventStore.Interface;
using CMA.ISMAI.Delivery.EventStore.Service;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Bus;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Camunda.Interface;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Camunda.Service;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.FileProcessing;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.FileReader;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Commands;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Events;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models;
using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.Logging.Service;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Mediator;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Reflection;

namespace CMA.ISMAI.Delivery.FileProcessing.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            Console.WriteLine(string.Format("File Processing is starting..! - {0}", DateTime.Now));
            serviceProvider.GetRequiredService<ConsoleApplication>().StartService();
            Console.ReadKey();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
               .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200/"))
               {
                   AutoRegisterTemplate = true,
              })
           .CreateLogger();

            services.AddLogging();
            services.AddScoped<ILoggingService, LoggingService>();

            services.AddScoped<IGenerateWaterMarkService, GenerateWaterMarkService>();
            services.AddScoped<IGenerateJuryPageService, GenerateJuryPageService>();
            services.AddScoped<ICoverPageService, CoverPageService>();
            services.AddScoped<IFileReaderService, FileReaderService>();
            services.AddScoped<ICamundaService, CamundaService>();
            services.AddScoped<IEventStoreService, EventStoreService>();
            services.AddScoped<IMediatorHandler, InMemoryBus>();
            services.AddScoped<INotificationService, NotificationService>();


            services.AddScoped<IRequestHandler<GenerateWaterMarkCommand, ValidationResult>, FileProcessingHandler>();
            services.AddScoped<IRequestHandler<GenerateCoverPageCommand, ValidationResult>, FileProcessingHandler>();
            services.AddScoped<IRequestHandler<GenerateJuryPageCommand, ValidationResult>, FileProcessingHandler>();

            services.AddScoped<INotificationHandler<WaterMarkGeneratedEvent>, FileProcessingEventHandler>();
            services.AddScoped<INotificationHandler<CoverPageGeneratedEvent>, FileProcessingEventHandler>();
            services.AddScoped<INotificationHandler<JuryPageGeneretedEvent>, FileProcessingEventHandler>();

            services.AddMediatR(typeof(Program).GetTypeInfo().Assembly);
            services.AddTransient<ConsoleApplication>();
            return services;
        }
    }
}
