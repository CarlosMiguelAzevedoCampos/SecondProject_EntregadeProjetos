using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CMA.ISMAI.Delivery.API.UI.IntegrationTests
{
    public class DeliveryFile_IntegrationTests
    {
        [Fact(DisplayName = "Invalid Request - No File Delivery")]
        [Trait("DeliveryController", "Submit a delivery - File Delivery")]
        public async Task NoFileDelivery()
        {
            // Arrange
            var builder = new WebHostBuilder()
                        .UseEnvironment("Development")
                        .UseStartup<Startup>();

            TestServer testServer = new TestServer(builder);

            HttpClient client = testServer.CreateClient();


            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(new StringContent(""), "StudentName");
            multiContent.Add(new StringContent(""), "InstituteName");
            multiContent.Add(new StringContent(""), "CourseName");
            multiContent.Add(new StringContent(""), "StudentEmail");
            multiContent.Add(new StringContent(""), "StudentNumber");
            multiContent.Add(new StringContent(""), "CordenatorName");
            multiContent.Add(new StringContent(""), "Title");
            multiContent.Add(new StringContent(""), "DefenitionOfDelivery");
            multiContent.Add(new StringContent(""), "PublicPDFVersionName");
            multiContent.Add(new StringContent(""), "PrivatePDFVersionName");
            var content = multiContent;

            // Act
            var response = await client.PostAsync("api/Deliveries/UploadWithFile", (HttpContent)content);
            var result = await response.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(result);

            // Assert
            Assert.Equal(400, resultObject.Status.Value);
            Assert.Single(resultObject.Errors["Messages"]);

        }

        [Fact(DisplayName = "Valid Request - File Delivery")]
        [Trait("DeliveryController", "Submit a delivery - File Delivery")]
        public async Task AGoodDeliveryDone()
        {
            // Arrange
            var builder = new WebHostBuilder()
                        .UseEnvironment("Development")
                        .UseStartup<Startup>();

            TestServer testServer = new TestServer(builder);

            HttpClient client = testServer.CreateClient();
            FormFile file = null;
            var stream = File.OpenRead(@"C:\Users\Carlos Campos\Desktop\A029217_ISMAI_Carlos_Informática.zip");
            
                file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/zip"
                };

            byte[] data;
            var br = new BinaryReader(file.OpenReadStream());
            data = br.ReadBytes((int)file.OpenReadStream().Length);

            ByteArrayContent bytes = new ByteArrayContent(data);


            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(bytes, "DeliveryFile", file.FileName);
            multiContent.Add(new StringContent("Carlos"), "StudentName");
            multiContent.Add(new StringContent("A029217"), "InstituteName");
            multiContent.Add(new StringContent("Informática"), "CourseName");
            multiContent.Add(new StringContent("a029216@ismai.pt"), "StudentEmail");
            multiContent.Add(new StringContent("a029216"), "StudentNumber");
            multiContent.Add(new StringContent("Alexandre Sousa"), "CordenatorName");
            multiContent.Add(new StringContent("BPMN"), "Title");
            multiContent.Add(new StringContent("Mestrado"), "DefenitionOfDelivery");
            multiContent.Add(new StringContent("public.pdf"), "PublicPDFVersionName");
            multiContent.Add(new StringContent("PrivateDelivery.pdf"), "PrivatePDFVersionName");

            // Act
            var response = await client.PostAsync("api/Deliveries/UploadWithFile", (HttpContent)multiContent);
            var result = await response.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<OkObjectResult>(result);

            // Assert
            Assert.Equal(200, resultObject.StatusCode);
        }

        [Fact(DisplayName = "Valid Request without file extesion - File Delivery")]
        [Trait("DeliveryController", "Submit a delivery - File Delivery")]
        public async Task ValidRequestWithOutPublicAndPrivateFileExtension()
        {
            // Arrange
            var builder = new WebHostBuilder()
                        .UseEnvironment("Development")
                        .UseStartup<Startup>();

            TestServer testServer = new TestServer(builder);

            HttpClient client = testServer.CreateClient();
            FormFile file = null;
            var stream = File.OpenRead(@"C:\Users\Carlos Campos\Desktop\A029217_ISMAI_Carlos_Informática.zip");

            file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/zip"
            };

            byte[] data;
            var br = new BinaryReader(file.OpenReadStream());
            data = br.ReadBytes((int)file.OpenReadStream().Length);

            ByteArrayContent bytes = new ByteArrayContent(data);


            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(bytes, "DeliveryFile", file.FileName);
            multiContent.Add(new StringContent("Carlos"), "StudentName");
            multiContent.Add(new StringContent("A029217"), "InstituteName");
            multiContent.Add(new StringContent("Informática"), "CourseName");
            multiContent.Add(new StringContent("a029216@ismai.pt"), "StudentEmail");
            multiContent.Add(new StringContent("a029216"), "StudentNumber");
            multiContent.Add(new StringContent("Alexandre Sousa"), "CordenatorName");
            multiContent.Add(new StringContent("BPMN"), "Title");
            multiContent.Add(new StringContent("Mestrado"), "DefenitionOfDelivery");
            multiContent.Add(new StringContent("public"), "PublicPDFVersionName");
            multiContent.Add(new StringContent("PrivateDelivery"), "PrivatePDFVersionName");

            // Act
            var response = await client.PostAsync("api/Deliveries/UploadWithFile", (HttpContent)multiContent);
            var result = await response.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<OkObjectResult>(result);

            // Assert
            Assert.Equal(200, resultObject.StatusCode);
        }


        [Fact(DisplayName = "File isn't a zip file")]
        [Trait("DeliveryController", "Submit a delivery - File Delivery")]
        public async Task FileIsntAZipFile()
        {
            // Arrange
            var builder = new WebHostBuilder()
                          .UseEnvironment("Development")
                          .UseStartup<Startup>();
            TestServer testServer = new TestServer(builder);
            HttpClient client = testServer.CreateClient();

            FormFile file = null;
            var stream = File.OpenRead(@"C:\Users\Carlos Campos\Desktop\segunda.docx");

            file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/zip"
            };

            byte[] data;
            var br = new BinaryReader(file.OpenReadStream());
            data = br.ReadBytes((int)file.OpenReadStream().Length);

            ByteArrayContent bytes = new ByteArrayContent(data);
            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(bytes, "DeliveryFile", file.FileName);
            multiContent.Add(new StringContent("Carlos"), "StudentName");
            multiContent.Add(new StringContent("A029217"), "InstituteName");
            multiContent.Add(new StringContent("Informática"), "CourseName");
            multiContent.Add(new StringContent("a029216@ismai.pt"), "StudentEmail");
            multiContent.Add(new StringContent("a029216"), "StudentNumber");
            multiContent.Add(new StringContent("Alexandre Sousa"), "CordenatorName");
            multiContent.Add(new StringContent("BPMN"), "Title");
            multiContent.Add(new StringContent("Mestrado"), "DefenitionOfDelivery");
            multiContent.Add(new StringContent("y.pdf"), "PublicPDFVersionName");
            multiContent.Add(new StringContent("x.pdf"), "PrivatePDFVersionName");


            // Act
            var response = await client.PostAsync("api/Deliveries/UploadWithFile", (HttpContent)multiContent);
            var result = await response.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(result);

            // Assert
            Assert.Equal(400, resultObject.Status.Value);
            Assert.Single(resultObject.Errors["Messages"]);
        }


    }
}
