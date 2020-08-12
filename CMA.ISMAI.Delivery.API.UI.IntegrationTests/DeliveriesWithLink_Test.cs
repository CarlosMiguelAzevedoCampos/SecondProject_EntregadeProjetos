using CMA.ISMAI.Delivery.API.UI.IntegrationTests.IntegrationModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CMA.ISMAI.Delivery.API.UI.IntegrationTests
{
    public class DeliveriesWithLink_Test
    {
        [Fact(DisplayName = "Invalid Request")]
        [Trait("DeliveryController", "Submit a delivery")]
        public async Task ABadDeliverieDone()
        {
            // Arrange
            var builder = new WebHostBuilder()
                        .UseEnvironment("Development")
                        .UseStartup<Startup_IntegrationTests>();

            TestServer testServer = new TestServer(builder);

            HttpClient client = testServer.CreateClient();
            var formData = new Dictionary<string, string>() { //<---- NOTE HERE
                {  "FileLink","" },
                {  "StudentName","" },
                {  "InstituteName","" },
                {  "CourseName","" },
                {  "StudentEmail","" },
                {  "StudentNumber","" },
                {  "CordenatorName","" },
                {  "Title","" },
                {  "DefenitionOfDelivery","" },
                {  "PublicPDFVersionName","" },
                {  "PrivatePDFVersionName","" }
            };

            // Act
            var content = new FormUrlEncodedContent(formData);
            var response = await client.PostAsync("api/Deliveries/UploadWithLink", content);
            var result = await response.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(result);

            // Assert
            Assert.Equal(400, resultObject.Status.Value);
            Assert.Equal(8, resultObject.Errors["Messages"].Length);

        }

        [Fact(DisplayName = "Valid Request")]
        [Trait("DeliveryController", "Submit a delivery")]
        public async Task AGoodDeliveryDone()
        {
            // Arrange
            var builder = new WebHostBuilder()
                        .UseEnvironment("Development")
                        .UseStartup<Startup_IntegrationTests>();

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
                {"PrivatePDFVersionName", "public" }
            };

            // Act
            var content = new FormUrlEncodedContent(formData);
            var response = await client.PostAsync("api/Deliveries/UploadWithLink", content);
            var result = await response.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<OkObjectResult>(result);

            // Assert
            Assert.Equal(200, resultObject.StatusCode.Value);
        }
    }
}
