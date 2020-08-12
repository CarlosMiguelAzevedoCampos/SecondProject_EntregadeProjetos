using AutoMapper;
using CMA.ISMAI.Delivery.API.CrossCutting.Bus;
using CMA.ISMAI.Delivery.API.CrossCutting.FileSaver;
using CMA.ISMAI.Delivery.API.CrossCutting.HttpRequest;
using CMA.ISMAI.Delivery.API.CrossCutting.IoC;
using CMA.ISMAI.Delivery.API.CrossCutting.Queue;
using CMA.ISMAI.Delivery.API.CrossCutting.Zip;
using CMA.ISMAI.Delivery.API.Domain.Commands.Handlers;
using CMA.ISMAI.Delivery.API.Domain.Commands.Models;
using CMA.ISMAI.Delivery.API.Domain.Events;
using CMA.ISMAI.Delivery.API.Domain.Events.Handlers;
using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetDevPack.Mediator;

namespace CMA.ISMAI.Delivery.API.UI.IntegrationTests.IntegrationModels
{
    public class Startup_IntegrationTests
    {
        public Startup_IntegrationTests(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // Domain Bus (Mediator)
            services.AddScoped<IMediatorHandler, InMemoryBus>();

            // Domain - Commands
            services.AddScoped<IRequestHandler<CreateDeliveryWithFileCommand, ValidationResult>, CreateDeliveryWithFileCommandHandler>();
            services.AddScoped<IRequestHandler<CreateDeliveryWithLinkCommand, ValidationResult>, CreateDeliveryWithLinkCommandHandler>();
            services.AddScoped<INotificationHandler<CreateDeliveryWithFileEvent>, CreateDeliveryWithFileEventHandler>();
            services.AddScoped<INotificationHandler<CreateDeliveryWithLinkEvent>, CreateDeliveryWithLinkEventHandler>();

            // Application - CrossCutting
            services.AddScoped<IZipFileService, ZipFromFileService>();
            services.AddScoped<IZipUrlService, ZipFromUrlService>();
            services.AddScoped<IHttpRequestService, HttpRequestService>();
            services.AddScoped<IQueueService, QueueService>();
            services.AddScoped<IFileSaverService, FileSaverService>();
            services.AddMediatR(typeof(Startup));
            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
