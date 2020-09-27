using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CMA.ISMAI.Delivery.API.UI.IntegrationTests
{
    public class DeliveriesWithLink_Test
    {
        [Fact(DisplayName = "Invalid Request - Link Delivery")]
        [Trait("DeliveryController", "Submit a delivery - Link Delivery")]
        public async Task ABadDeliverieDone()
        {
            // Arrange
            var builder = new WebHostBuilder()
                        .UseEnvironment("Development")
                        .UseStartup<Startup>();

            TestServer testServer = new TestServer(builder);

            HttpClient client = testServer.CreateClient();
            var formData = new Dictionary<string, string>() { //<---- NOTE HERE
                { "StudentName", "" },
                {"InstituteName", "" },
                {"CourseName", "" },
                {"StudentEmail", "" },
                {"StudentNumber", "" },
                {"FileLink", "" },
                {"CordenatorName", "" },
                {"Title", "" },
                {"DefenitionOfDelivery", "" },
                {"PublicPDFVersionName", "" },
                {"PrivatePDFVersionName", "" },
                {"DeliveryTime", DateTime.Now.ToString() }
            };

            // Act
            var content = new FormUrlEncodedContent(formData);
            var response = await client.PostAsync("/api/Deliveries/UploadWithLink", content);
            var result = await response.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(result);

            // Assert
            Assert.Equal(400, resultObject.Status.Value);
            Assert.Equal(9, resultObject.Errors["Messages"].Length);

        }

        [Fact(DisplayName = "Valid Request - Link Delivery")]
        [Trait("DeliveryController", "Submit a delivery - Link Delivery")]
        public async Task AGoodDeliveryDone()
        {
            // Arrange
            var builder = new WebHostBuilder()
                          .UseEnvironment("Development")
                          .UseStartup<Startup>();
            TestServer testServer = new TestServer(builder);
            HttpClient client = testServer.CreateClient();

            var formData = new Dictionary<string, string>() { //<---- NOTE HERE
                { "StudentName", "Carlops" },
                {"InstituteName", "ISMAI" },
                {"CourseName", "Informática" },
                {"StudentEmail", "carlosmiguelcampos1996@gmail.com" },
                {"StudentNumber", "A029216" },
                {"FileLink", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=vjXWy1" },
                {"CordenatorName", "José" },
                {"Title", "Entrega de Dissertação 210" },
                {"DefenitionOfDelivery", "Mestrado" },
                {"PublicPDFVersionName", "PrivateProjectDelivery.pdf" },
                {"PrivatePDFVersionName", "public.pdf" },
                {"DeliveryTime", DateTime.Now.ToString() }
            };



            // Act
            var content = new FormUrlEncodedContent(formData);
            var response = await client.PostAsync("/api/Deliveries/UploadWithLink", content);
            var result = await response.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<OkObjectResult>(result);

            // Assert
            Assert.Equal(200, resultObject.StatusCode.Value);
        }

        [Fact(DisplayName = "Valid Request without file extension - Link Delivery")]
        [Trait("DeliveryController", "Submit a delivery - Link Delivery")]
        public async Task AGoodDeliveryDoneWithoutFileExtension()
        {
            // Arrange
            var builder = new WebHostBuilder()
                          .UseEnvironment("Development")
                          .UseStartup<Startup>();
            TestServer testServer = new TestServer(builder);
            HttpClient client = testServer.CreateClient();

            var formData = new Dictionary<string, string>() { //<---- NOTE HERE
                { "StudentName", "Carlops" },
                {"InstituteName", "ISMAI" },
                {"CourseName", "Informática" },
                {"StudentEmail", "carlosmiguelcampos1996@gmail.com" },
                {"StudentNumber", "A029216" },
                {"FileLink", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=vjXWy1" },
                {"CordenatorName", "José" },
                {"Title", "Entrega de Dissertação 210" },
                {"DefenitionOfDelivery", "Mestrado" },
                {"PublicPDFVersionName", "PrivateProjectDelivery" },
                {"PrivatePDFVersionName", "public" },
                {"DeliveryTime", DateTime.Now.ToString() }
            };



            // Act
            var content = new FormUrlEncodedContent(formData);
            var response = await client.PostAsync("/api/Deliveries/UploadWithLink", content);
            var result = await response.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<OkObjectResult>(result);

            // Assert
            Assert.Equal(200, resultObject.StatusCode.Value);
        }

        [Fact(DisplayName = "Invalid link Request - Link Delivery")]
        [Trait("DeliveryController", "Submit a delivery - Link Delivery")]
        public async Task LinkIsntfromaTrustedHost()
        {
            // Arrange
            var builder = new WebHostBuilder()
                          .UseEnvironment("Development")
                          .UseStartup<Startup>();
            TestServer testServer = new TestServer(builder);
            HttpClient client = testServer.CreateClient();

            var formData = new Dictionary<string, string>() { //<---- NOTE HERE
                { "StudentName", "Carlops" },
                {"InstituteName", "ISMAI" },
                {"CourseName", "Informática" },
                {"StudentEmail", "carlosmiguelcampos1996@gmail.com" },
                {"StudentNumber", "A029216" },
                {"FileLink", "https://meocloud.pt/link/8ea5686b-aba1-43b5-9f7c-c41bd0075eed/New%20Compressed%20%28zipped%29%20Folder.zip/" },
                {"CordenatorName", "José" },
                {"Title", "Entrega de Dissertação 210" },
                {"DefenitionOfDelivery", "Mestrado" },
                {"PublicPDFVersionName", "PrivateProjectDelivery" },
                {"PrivatePDFVersionName", "public" },
                {"DeliveryTime", DateTime.Now.ToString() }
            };



            // Act
            var content = new FormUrlEncodedContent(formData);
            var response = await client.PostAsync("/api/Deliveries/UploadWithLink", content);
            var result = await response.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(result);

            // Assert
            Assert.Equal(400, resultObject.Status.Value);
        }


        [Fact(DisplayName = "File isnt a ZIP file - Link Delivery")]
        [Trait("DeliveryController", "Submit a delivery - Link Delivery")]
        public async Task FileIsntAZipFile()
        {
            // Arrange
            var builder = new WebHostBuilder()
                          .UseEnvironment("Development")
                          .UseStartup<Startup>();
            TestServer testServer = new TestServer(builder);
            HttpClient client = testServer.CreateClient();

            var formData = new Dictionary<string, string>() { //<---- NOTE HERE
                { "StudentName", "Carlops" },
                {"InstituteName", "ISMAI" },
                {"CourseName", "Informática" },
                {"StudentEmail", "carlosmiguelcampos1996@gmail.com" },
                {"StudentNumber", "A029216" },
                {"FileLink", "https://1drv.ms/t/s!Aos6ApXpMWOBa6LAqlkptQgrRvQ?e=wcWClI" },
                {"CordenatorName", "José" },
                {"Title", "Entrega de Dissertação 210" },
                {"DefenitionOfDelivery", "Mestrado" },
                {"PublicPDFVersionName", "PrivateProject" },
                {"PrivatePDFVersionName", "publicProject" },
                {"DeliveryTime", DateTime.Now.ToString() }
            };



            // Act
            var content = new FormUrlEncodedContent(formData);
            var response = await client.PostAsync("/api/Deliveries/UploadWithLink", content);
            var result = await response.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(result);

            // Assert
            Assert.Equal(400, resultObject.Status.Value);
        }
    }
}
