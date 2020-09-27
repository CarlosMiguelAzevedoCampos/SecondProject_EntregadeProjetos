using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.UI.HealthCheck.Interface;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.UI.HealthCheck.Service
{
    public class HttpRequest : IHttpRequest
    {
        private readonly ILoggingService _log;
        public HttpRequest(ILoggingService log)
        {
            _log = log;
        }
        public async Task<HttpResponseMessage> MakeAnHttpRequest(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                var result = await client.GetAsync(url);
                return result;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
