using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.UI.HealthCheck.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.UI.HealthCheck
{
    public class CamundaHealthCheck : IHealthCheck
    {
        private readonly ILoggingService _log;
        private readonly IHttpRequest _httpRequest;
        private readonly IConfiguration _config;

        public CamundaHealthCheck(ILoggingService log,
                                  IHttpRequest httpRequest)
        {
            _log = log;
            _config = new ConfigurationBuilder()
                                                                .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                                                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                                .AddEnvironmentVariables()
                                                                .Build();
            this._httpRequest = httpRequest;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var result = await _httpRequest.MakeAnHttpRequest(_config.GetSection("Camunda:Uri").Value);
                return result.IsSuccessStatusCode ? HealthCheckResult.Healthy("The API is working fine!") :
                                                        HealthCheckResult.Unhealthy("The API is DOWN!");
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
                return HealthCheckResult.Unhealthy("The API is DOWN!");
            }
        }
    }
}