using System.Net.Http;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.UI.HealthCheck.Interface
{
    public interface IHttpRequest
    {
        Task<HttpResponseMessage> MakeAnHttpRequest(string url);
    }
}
