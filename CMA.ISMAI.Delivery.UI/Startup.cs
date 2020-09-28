using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.Logging.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;

namespace CMA.ISMAI.Delivery.UI
{
    public class Startup
    {
        private readonly IWebHostEnvironment _currentEnvironment;
        public Startup(IWebHostEnvironment env)
        {
            _currentEnvironment = env;
            Configuration = new ConfigurationBuilder()
                                                   .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                                                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables()
                                                   .Build();
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(Configuration.GetSection("ElasticConfiguration:Uri").Value))
            {
                AutoRegisterTemplate = true,
            })
               .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ILoggingService, LoggingService>();

            services.AddControllersWithViews();
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
