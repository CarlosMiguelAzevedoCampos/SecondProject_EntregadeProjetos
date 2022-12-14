using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using System;
using System.IO;
using System.Net;

namespace CMA.ISMAI.Delivery.API.CrossCutting.HttpRequest
{
    public class HttpRequestService : IHttpRequestService
    {
        private readonly ILoggingService _log;

        public HttpRequestService(ILoggingService log)
        {
            _log = log;
        }


        public HttpWebResponse ReturnObjectFromTheUrl(string url)
        {
            try
            {
                return ReturnWebResponse(url);
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return null;
        }


        public HttpWebResponse ReturnFileInformation(HttpWebResponse url)
        {
            try
            {
                var responseUriResult = ReturnWebResponse(url.ResponseUri.AbsoluteUri.Replace("redir", "download"));
                return responseUriResult;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return null;
        }

        private HttpWebResponse ReturnWebResponse(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        public bool VerifyIfLinkIsFromTheTrustedHoster(HttpWebResponse webReponse)
        {
            try
            {
                return webReponse.ResponseUri.Host == "onedrive.live.com";
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        public bool IsAZipFile(HttpWebResponse webReponse)
        {
            try
            {
                return webReponse.ContentType == "application/zip";
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        public bool IsTheFileSmallerThanFiveGB(HttpWebResponse webReponse)
        {
            try
            {
                return webReponse.ContentLength <= 5000000000;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return true;
        }
    }
}
