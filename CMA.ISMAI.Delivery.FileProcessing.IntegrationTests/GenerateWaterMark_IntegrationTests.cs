using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Core.Service;
using CMA.ISMAI.Delivery.EventStore.Interface;
using CMA.ISMAI.Delivery.EventStore.Service;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Bus;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Camunda.Interface;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Camunda.Service;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.FileMover;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.FileProcessing;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.FileReader;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Commands;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Events;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models.Commands;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models.Events;
using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.Logging.Service;
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

namespace CMA.ISMAI.Delivery.FileProcessing.IntegrationTests
{
    public class GenerateWaterMark_IntegrationTests
    {
        [Fact(DisplayName = "Valid water mark generation")]
        [Trait("GenerateWaterMarkCommand", "Generate Water Mark - Integration Tests")]
        public async Task GenerateCover()
        {
            // Arrange
            File.Delete(@"C:\Users\Carlos Campos\Desktop\Teste\Unzip\FileProcessing\A029216_ISMAI_Carlos Campos_Informática_WaterMarkTest\PrivateWaterMark.pdf");
            File.Delete(@"C:\Users\Carlos Campos\Desktop\Teste\Unzip\FileProcessing\A029216_ISMAI_Carlos Campos_Informática_WaterMarkTest\PublicWaterMark.pdf");
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            var generateWaterMark = new GenerateWaterMarkCommand("A029216", @"C:\Users\Carlos Campos\Desktop\Teste\Unzip\FileProcessing\A029216_ISMAI_Carlos Campos_Informática_WaterMarkTest\", "Informmática", "ISMAI", DateTime.Now,
                "public.pdf", "PrivateProjectDelivery.pdf");
            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(generateWaterMark);
            //Assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Fail water mark generation - Private file")]
        [Trait("GenerateWaterMarkCommand", "Generate Water Mark - Integration Tests")]
        public async Task FailWaterMarkGeneration_PrivateFileNotFound()
        {
            // Arrange
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            var generateWaterMark = new GenerateWaterMarkCommand("A029216", @"C:\Users\Carlos Campos\Desktop\Teste\Unzip\FileProcessing\A029216_ISMAI_Carlos Campos_Informática_WaterMarkTest\", "Informmática", "ISMAI", DateTime.Now,
                "public.pdf", "private.pdf");
            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(generateWaterMark);
            //Assert
            Assert.False(result.IsValid);
        }


        [Fact(DisplayName = "Fail water mark generation - Public file")]
        [Trait("GenerateWaterMarkCommand", "Generate Water Mark - Integration Tests")]
        public async Task FailWaterMarkGeneration_PublicFileNotFound()
        {
            // Arrange
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            var generateWaterMark = new GenerateWaterMarkCommand("A029216", @"C:\Users\Carlos Campos\Desktop\Teste\Unzip\FileProcessing\A029216_ISMAI_Carlos Campos_Informática_WaterMarkTest\", "Informmática", "ISMAI", DateTime.Now,
                "PublicDelivery.pdf", "PrivateProjectDelivery.pdf");
            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(generateWaterMark);
            //Assert
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

            services.AddScoped<IGenerateWaterMarkService, GenerateWaterMarkService>();
            services.AddScoped<IGenerateJuryPageService, GenerateJuryPageService>();
            services.AddScoped<ICoverPageService, CoverPageService>();
            services.AddScoped<IFileReaderService, FileReaderService>();
            services.AddScoped<ICamundaService, CamundaService>();
            services.AddScoped<IEventStoreService, EventStoreService>();
            services.AddScoped<IMediatorHandler, InMemoryBus>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IFileTransferService, FileTransferService>();


            services.AddScoped<IRequestHandler<GenerateWaterMarkCommand, ValidationResult>, FileProcessingHandler>();
            services.AddScoped<IRequestHandler<GenerateCoverPageCommand, ValidationResult>, FileProcessingHandler>();
            services.AddScoped<IRequestHandler<GenerateJuryPageCommand, ValidationResult>, FileProcessingHandler>();
            services.AddScoped<IRequestHandler<FileTransferCommand, ValidationResult>, FileProcessingHandler>();

            services.AddScoped<INotificationHandler<WaterMarkGeneratedEvent>, FileProcessingEventHandler>();
            services.AddScoped<INotificationHandler<CoverPageGeneratedEvent>, FileProcessingEventHandler>();
            services.AddScoped<INotificationHandler<JuryPageGeneretedEvent>, FileProcessingEventHandler>();
            services.AddScoped<INotificationHandler<FileTransferCompletedEvent>, FileProcessingEventHandler>();

            services.AddMediatR(typeof(GenerateCover_IntegrationTests).GetTypeInfo().Assembly);
            return services;
        }
    }
}
