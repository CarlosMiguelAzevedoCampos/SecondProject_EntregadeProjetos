using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Core.Service;
using CMA.ISMAI.Delivery.EventStore.Interface;
using CMA.ISMAI.Delivery.EventStore.Service;
using CMA.ISMAI.Delivery.FileLoading.Bus;
using CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileDownload;
using CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileIdentifier;
using CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileVerifier;
using CMA.ISMAI.Delivery.FileLoading.CrossCutting.Queue;
using CMA.ISMAI.Delivery.FileLoading.Domain.Commands;
using CMA.ISMAI.Delivery.FileLoading.Domain.Events;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model.Events;
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

namespace CMA.ISMAI.Delivery.FileLoading.IntegrationTests
{
    public class FileIdentifier_IntegrationTests
    {
        [Fact(DisplayName = "Identifiers created")]
        [Trait("CreateFileIdentifiersCommand", "FileIdentifiers - Integration Tests")]
        public async Task IdentifiersCreated()
        {
            // Arrange
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            CreateFileIdentifiersCommand createFileIdentifiersCommand = new CreateFileIdentifiersCommand(Guid.NewGuid(), @"C:\Users\Carlos Campos\Desktop\Teste\Unzip\A029216_ISMAI_Carlos Campos_Informática", "a029216@ismai.pt", "carlosmiguelcampos1996@gmail.com");

            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(createFileIdentifiersCommand);
            //Assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Identifiers failed to be created")]
        [Trait("CreateFileIdentifiersCommand", "FileIdentifiers - Integration Tests")]
        public async Task FilePathNotFound()
        {
            // Arrange
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            CreateFileIdentifiersCommand createFileIdentifiersCommand = new CreateFileIdentifiersCommand(Guid.NewGuid(), @"C:\Users\Carlos Campos\Desktop\Teste\Unzip\A029216_ISMAI_Carlos Campos_Informátic","a029216@ismai.pt", "carlosmiguelcampos1996@gmail.com");

            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(createFileIdentifiersCommand);
            //Assert
            Assert.False(result.IsValid);
        }

        private IServiceCollection ConfigureServices()
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
            services.AddScoped<IHttpRequestService, FileDownloadService>();
            services.AddScoped<IFileVerifierService, FileVerifierService>();
            services.AddScoped<IFileIdentifierService, FileIdentifierService>();
            services.AddScoped<IVerifyFilesExtensions, VerifyFilesExtensions>();
            services.AddScoped<IPDFVerifierService, PDFVerifierService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEventStoreService, EventStoreService>();
            services.AddScoped<IQueueService, QueueService>();

            services.AddScoped<IRequestHandler<DownloadFileFromUrlCommand, ValidationResult>, DownloadFileCommandHandler>();
            services.AddScoped<IRequestHandler<CreateFileIdentifiersCommand, ValidationResult>, FileIdentifiersCommandHandler>();
            services.AddScoped<IRequestHandler<VerifyFilesCommand, ValidationResult>, VerifyFileCommandHandler>();

            services.AddScoped<INotificationHandler<FileDownloadedEvent>, FileLoadingEventHandler>();
            services.AddScoped<INotificationHandler<FilesIdentifiedEvent>, FileLoadingEventHandler>();
            services.AddScoped<INotificationHandler<FilesVerifiedEvent>, FileLoadingEventHandler>();
            services.AddMediatR(typeof(DownloadFileFromUrl_IntegrationTests).GetTypeInfo().Assembly);


            return services;
        }
    }
}
