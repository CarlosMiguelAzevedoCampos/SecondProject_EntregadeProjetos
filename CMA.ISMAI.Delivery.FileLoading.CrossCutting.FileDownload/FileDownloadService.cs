using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using System;
using System.Net;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileDownload
{
    public class FileDownloadService : IHttpRequestService
    {
        private readonly ILoggingService _log;

        public FileDownloadService(ILoggingService log)
        {
            _log = log;
        }
        public bool DownloadFileToHost(string pathToSave, string fileUrl)
        {
            try
            {
                var webResponse = ReturnWebResponse(fileUrl);
                using (WebClient webClient = new WebClient())
                {
                    fileUrl = webResponse.ResponseUri.AbsoluteUri.Replace("redir", "download");
                    webClient.DownloadFile(fileUrl, pathToSave);
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        private HttpWebResponse ReturnWebResponse(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }
    }
}
