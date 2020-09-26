using CMA.ISMAI.Delivery.Logging.Interface;
using Microsoft.Extensions.Logging;

namespace CMA.ISMAI.Delivery.Logging.Service
{
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        // constructor
        public LoggingService(ILogger<LoggingService> logger = null)
        {
            _logger = logger;
        }

        public void Fatal(string message)
        {
            _logger.LogError($"Please, take care of this as soon as possible! - {message}");
        }

        public void Info(string message)
        {
            _logger.LogInformation($"Just a information.., look! - {message}");
        }
    }
}
