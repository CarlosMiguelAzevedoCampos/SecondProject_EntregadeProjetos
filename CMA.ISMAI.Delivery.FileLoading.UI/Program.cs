using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Core.Service;
using CMA.ISMAI.Delivery.FileLoading.Bus;
using CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda.Interface;
using CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda.Service;
using CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileDownload;
using CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileIdentifier;
using CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileVerifier;
using CMA.ISMAI.Delivery.FileLoading.CrossCutting.Queue;
using CMA.ISMAI.Delivery.FileLoading.Domain.Commands;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
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

namespace CMA.ISMAI.Delivery.FileLoading.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            Console.WriteLine(string.Format("File Loading is starting..! - {0}", DateTime.Now));
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

            services.AddScoped<IMediatorHandler, InMemoryBus>();
            services.AddScoped<IHttpRequestService, FileDownloadService>();
            services.AddScoped<IFileVerifierService, FileVerifierService>();
            services.AddScoped<IFileIdentifierService, FileIdentifierService>();
            services.AddScoped<IMediaFileVerifierService, MediaFileVerifierService>();
            services.AddScoped<IPDFVerifierService, PDFVerifierService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICamundaService, CamundaService>();
            services.AddScoped<IQueueService, QueueService>();

            services.AddScoped<IRequestHandler<DownloadFileFromUrlCommand, ValidationResult>, DownloadFileCommandHandler>();
            services.AddScoped<IRequestHandler<CreateFileIdentifiersCommand, ValidationResult>, FileIdentifiersCommandHandler>();
            services.AddScoped<IRequestHandler<VerifyFilesCommand, ValidationResult>, VerifyFileCommandHandler>();
            
            services.AddMediatR(typeof(Program).GetTypeInfo().Assembly);
            services.AddTransient<ConsoleApplication>();
            return services;
        }
    }
}
