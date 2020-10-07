using AutoMapper;
using CMA.ISMAI.Delivery.API.CrossCutting.IoC;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;

namespace CMA.ISMAI.Delivery.API.UI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IWebHostEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                                          .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables()
                                          .Build();
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(Configuration.GetSection("ElasticConfiguration:Uri").Value))
            {
                AutoRegisterTemplate = true,
                IndexFormat = "DeliveryISMAIAPI"
            })
               .CreateLogger();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            NativeInjectorBootStrapper.RegisterServices(services);
            services.AddMediatR(typeof(Startup));
            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            loggerFactory.AddSerilog();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
