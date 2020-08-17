using CMA.ISMAI.Delivery.API.CrossCutting.Bus;
using CMA.ISMAI.Delivery.API.CrossCutting.FileSaver;
using CMA.ISMAI.Delivery.API.CrossCutting.HttpRequest;
using CMA.ISMAI.Delivery.API.CrossCutting.Queue;
using CMA.ISMAI.Delivery.API.CrossCutting.Zip;
using CMA.ISMAI.Delivery.API.Domain.Commands.Handlers;
using CMA.ISMAI.Delivery.API.Domain.Commands.Models;
using CMA.ISMAI.Delivery.API.Domain.Events;
using CMA.ISMAI.Delivery.API.Domain.Events.Handlers;
using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using CMA.ISMAI.Delivery.EventStore.Interface;
using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.Logging.Service;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Mediator;

namespace CMA.ISMAI.Delivery.API.CrossCutting.IoC
{
    public static class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Domain Bus (Mediator)
            services.AddScoped<IMediatorHandler, InMemoryBus>();

            // Domain - Commands
            services.AddScoped<IRequestHandler<CreateDeliveryWithFileCommand, ValidationResult>, CreateDeliveryWithFileCommandHandler>();
            services.AddScoped<IRequestHandler<CreateDeliveryWithLinkCommand, ValidationResult>, CreateDeliveryWithLinkCommandHandler>();
            services.AddScoped<INotificationHandler<CreateDeliveryWithFileEvent>, CreateDeliveryWithFileEventHandler>();
            services.AddScoped<INotificationHandler<CreateDeliveryWithLinkEvent>, CreateDeliveryWithLinkEventHandler>();

            // Application - CrossCutting
            services.AddScoped<IZipFileService, ZipFromFileService>();
            services.AddScoped<IEventStoreService, EventStore.Service.EventStoreService>();
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<IZipUrlService, ZipFromUrlService>();
            services.AddScoped<IHttpRequestService, HttpRequestService>();
            services.AddScoped<IQueueService, QueueService>();
            services.AddScoped<IFileSaverService, FileSaverService>();
        }
    }
}
