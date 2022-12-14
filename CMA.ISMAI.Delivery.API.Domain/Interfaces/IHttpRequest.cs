using System.IO;
using System.Net;

namespace CMA.ISMAI.Delivery.API.Domain.Interfaces
{
    public interface IHttpRequestService
    {
        HttpWebResponse ReturnObjectFromTheUrl(string url);
        HttpWebResponse ReturnFileInformation(HttpWebResponse httpWebRequest);
        bool VerifyIfLinkIsFromTheTrustedHoster(HttpWebResponse webReponse);
        bool IsAZipFile(HttpWebResponse webReponse);
        bool IsTheFileSmallerThanFiveGB(HttpWebResponse webReponse);
    }
}
