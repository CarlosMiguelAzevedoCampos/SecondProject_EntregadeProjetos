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
    public class GenerateJuryPage_IntegrationTests
    {
        [Fact(DisplayName = "Valid jury page generation")]
        [Trait("GenerateJuryPageCommand", "Generate Jury page")]
        public async Task GenerateCover()
        {

            File.Delete(@"C:\Users\Carlos Campos\Desktop\Teste\Unzip\FileProcessing\A029216_ISMAI_Carlos Campos_Informática_JuryPageTest\FinalPrivate_Delivery.pdf");
            File.Delete(@"C:\Users\Carlos Campos\Desktop\Teste\Unzip\FileProcessing\A029216_ISMAI_Carlos Campos_Informática_JuryPageTest\FinalPublic_Delivery.pdf");
            // Arrange
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            var generateJuryPageCommand = new GenerateJuryPageCommand("Carlos Campos", "A029216", "Informática", "ISMAI", @"C:\Users\Carlos Campos\Desktop\Teste\Unzip\FileProcessing\A029216_ISMAI_Carlos Campos_Informática_JuryPageTest", "C:\\Users\\Carlos Campos\\Desktop\\ismai.xlsx");

            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(generateJuryPageCommand);
            //Assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Path not found for jury page generation")]
        [Trait("GenerateJuryPageCommand", "Generate Jury page")]
        public async Task FailToFindPath()
        {
           // Arrange
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            var generateJuryPageCommand = new GenerateJuryPageCommand("Carlos Campos", "A029216", "Informática", "ISMAI", @"C:\Users\Carlos Campos\Desktop\Teste\Unzip\FileProcessing\A039216_ISMAI_Carlos Campos_Informática_JuryPageTest", "C:\\Users\\Carlos Campos\\Desktop\\ismai.xlsx");

            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(generateJuryPageCommand);
            //Assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "Jury not found")]
        [Trait("GenerateJuryPageCommand", "Generate Jury page")]
        public async Task JuryNotFound()
        {
            // Arrange
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            var generateJuryPageCommand = new GenerateJuryPageCommand("Carlos Campos", "A039216", "Informática", "ISMAI", @"C:\Users\Carlos Campos\Desktop\Teste\Unzip\FileProcessing\A029216_ISMAI_Carlos Campos_Informática_JuryPageTest", "C:\\Users\\Carlos Campos\\Desktop\\ismai.xlsx");

            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(generateJuryPageCommand);
            //Assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "Jury file not found")]
        [Trait("GenerateJuryPageCommand", "Generate Jury page")]
        public async Task JuryFileNotFound()
        {
            // Arrange
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            var generateJuryPageCommand = new GenerateJuryPageCommand("Carlos Campos", "A029216", "Informática", "ISMAI", @"C:\Users\Carlos Campos\Desktop\Teste\Unzip\FileProcessing\A029216_ISMAI_Carlos Campos_Informática_JuryPageTest", "C:\\Users\\Carlos Campos\\Desktop\\ismaiJury.xlsx");

            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(generateJuryPageCommand);
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


            services.AddScoped<IRequestHandler<GenerateWaterMarkCommand, ValidationResult>, FileProcessingHandler>();
            services.AddScoped<IRequestHandler<GenerateCoverPageCommand, ValidationResult>, FileProcessingHandler>();
            services.AddScoped<IRequestHandler<GenerateJuryPageCommand, ValidationResult>, FileProcessingHandler>();

            services.AddScoped<INotificationHandler<WaterMarkGeneratedEvent>, FileProcessingEventHandler>();
            services.AddScoped<INotificationHandler<CoverPageGeneratedEvent>, FileProcessingEventHandler>();
            services.AddScoped<INotificationHandler<JuryPageGeneretedEvent>, FileProcessingEventHandler>();

            services.AddMediatR(typeof(GenerateCover_IntegrationTests).GetTypeInfo().Assembly);
            return services;
        }

    }
}
