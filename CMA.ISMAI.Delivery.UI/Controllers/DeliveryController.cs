using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.UI.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggingService _log;

        public DeliveryController(IConfiguration configuration, ILoggingService log)
        {
            _configuration = configuration;
            _log = log;
        }


        // GET: Delivery/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Delivery/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(53428800)]
        public async Task<ActionResult> Create([FromForm]DeliveryDto collection)
        {

            try
            {
                using (var client = new HttpClient())
                {
                    object content = null;
                    string url = string.Empty;
                    if (collection.DeliveryType)
                    {
                        var formData = new Dictionary<string, string>() { //<---- NOTE HERE
                            { "StudentName", collection.StudentName },
                            {"InstituteName", collection.InstituteName },
                            {"CourseName", collection.CourseName },
                            {"StudentEmail", collection.StudentEmail },
                            {"StudentNumber", collection.StudentNumber},
                            {"FileLink", collection.FileUrl },
                            {"CordenatorName", collection.CordenatorName },
                            {"Title", collection.Title },
                            {"DefenitionOfDelivery", collection.DefenitionOfDelivery },
                            {"PublicPDFVersionName", collection.PublicPDFVersionName},
                            {"PrivatePDFVersionName", collection.PrivatePDFVersionName },
                            {"DeliveryTime", DateTime.Now.ToString() }
                        };

                        // Act
                        content = new FormUrlEncodedContent(formData);
                        url = _configuration.GetSection("API_File_URL:Uri").Value.ToString();
                    }
                    else
                    {
                        byte[] data;
                        using (var br = new BinaryReader(collection.FormFile.OpenReadStream()))
                            data = br.ReadBytes((int)collection.FormFile.OpenReadStream().Length);

                        ByteArrayContent bytes = new ByteArrayContent(data);


                        MultipartFormDataContent multiContent = new MultipartFormDataContent();

                        multiContent.Add(bytes, "DeliveryFile", collection.FormFile.FileName);
                        multiContent.Add(new StringContent(collection.StudentName), "StudentName");
                        multiContent.Add(new StringContent(collection.InstituteName), "InstituteName");
                        multiContent.Add(new StringContent(collection.CourseName), "CourseName");
                        multiContent.Add(new StringContent(collection.StudentEmail), "StudentEmail");
                        multiContent.Add(new StringContent(collection.StudentNumber), "StudentNumber");
                        multiContent.Add(new StringContent(collection.CordenatorName), "CordenatorName");
                        multiContent.Add(new StringContent(collection.Title), "Title");
                        multiContent.Add(new StringContent(collection.DefenitionOfDelivery), "DefenitionOfDelivery");
                        multiContent.Add(new StringContent(collection.PublicPDFVersionName), "PublicPDFVersionName");
                        multiContent.Add(new StringContent(collection.PrivatePDFVersionName), "PrivatePDFVersionName");
                        multiContent.Add(new StringContent(DateTime.Now.ToString()), "DeliveryTime");
                        content = multiContent;
                        url = _configuration.GetSection("API_File_ZIP:Uri").Value.ToString();
                    }
                    var response = await client.PostAsync(url, (HttpContent)content);
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var resultObject = JsonConvert.DeserializeObject<OkObjectResult>(result);
                        TempData["result"] = "Projeto entregue!";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var resultObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(result);
                        TempData["bad_result"] = resultObject.Errors["Messages"];
                    }
                    else
                    {
                        TempData["unkn_result"] = "Algo falhou, porfavor, contacta a universidade";
                    }
                    return View();
                }
            }
            catch(Exception ex)
            {
                this._log.Fatal(ex.ToString());
                TempData["unkn_result"] = "Algo falhou, porfavor, contacta a universidade";
                return View();
            }

        }
    }
}