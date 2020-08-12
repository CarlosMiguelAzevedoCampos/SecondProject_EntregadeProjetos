using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using System;
using System.Net;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileDownload
{
    public class FileDownloadService : IHttpRequestService
    {
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
