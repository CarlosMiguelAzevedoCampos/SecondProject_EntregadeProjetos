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
    public class DownloadFileFromUrl_IntegrationTests
    {
        [Fact(DisplayName = "Valid Download Command")]
        [Trait("DownloadFileFromUrlCommand", "Download Command phase")]
        public async Task DonwloadHasBeenDone()
        {
            // Arrange
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            DownloadFileFromUrlCommand downloadCommand = new DownloadFileFromUrlCommand(Guid.Parse(Guid.NewGuid().ToString())
                       , "Carlos Campos", "ISMAI", "Informática", "a029216@ismai.pt", Guid.NewGuid().ToString(), DateTime.Now,
                      "José", "Mestrado", "Exemplo de título", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=7FQK97", "public.pdf ", "PrivateProjectDelivery.pdf");
            // Act
            var result = await serviceProvider.GetRequiredService<IMediator>().Send(downloadCommand);
            //Assert
            Assert.True(result.IsValid);
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
            services.AddScoped<IHttpRequestService, FileDownloadService>();
            services.AddScoped<IFileVerifierService, FileVerifierService>();
            services.AddScoped<IFileIdentifierService, FileIdentifierService>();
            services.AddScoped<IMediaFileVerifierService, MediaFileVerifierService>();
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
