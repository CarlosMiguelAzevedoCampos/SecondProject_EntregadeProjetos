using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetDevPack.Mediator;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.Payment.UI
{
    internal class ConsoleApplication
    {
        private readonly ICamundaService _camundaService;
        private readonly INotificationService _notificationService;
        private readonly ILoggingService _log;
        private readonly IConfiguration _config;
        public ConsoleApplication(ICamundaService camundaService, ILoggingService log, INotificationService notificationService)
        {
            _camundaService = camundaService;
            _log = log;
            _notificationService = notificationService;
            _config = new ConfigurationBuilder()
                                                      .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                                                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                      .AddEnvironmentVariables()
                                                      .Build();
        }

        public async Task StartServiceAsync()
        {
            try
            {
                _log.Info($"Payment is starting.. {DateTime.Now}");
                _camundaService.RegistWorkers();
                var factory = new ConnectionFactory()
                {
                    HostName = _config.GetSection("RabbitMqCore:Uri").Value,
                    Port = Convert.ToInt32(_config.GetSection("RabbitMqCore:Port").Value),
                    UserName = _config.GetSection("RabbitMqCore:Username").Value,
                    Password = _config.GetSection("RabbitMqCore:Password").Value
                };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "PaymentProcessing",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;
                    channel.BasicConsume(queue: "PaymentProcessing",
                         autoAck: true,
                         consumer: consumer);
                    var hostBuilder = new HostBuilder();
                    await hostBuilder.RunConsoleAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}, RabbitMQ starting..? ", ex.ToString()));
                Console.WriteLine("Retrying in 30 seconds..");
                _log.Fatal(string.Format("{0}, RabbitMQ starting..? ", ex.ToString()));
                Thread.Sleep(30000);
                await StartServiceAsync();
            }
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var notification = JsonConvert.DeserializeObject
                    (Encoding.UTF8.GetString(e.Body.ToArray()), settings);
            try
            {
                var parseObject = (Core.Model.Delivery)notification;
                if (!_camundaService.StartWorkFlow(parseObject))
                {
                    _notificationService.SendEmail(_config.GetSection("Notification:Email").Value, string.Format("Olá!, <br/> <br/> Uma entrega falhou a inicar no Diagrama Payment! <br/> <br/>  Os items são: " +
                        "<br/> <br/> Nome: {0} <br/> <br/> Número de Aluno {1} <br/> <br/> Curso: {2} <br/> <br/> Aqui tem o JSON com a informação {3} <br/><br/> Obrigado.", parseObject.StudentName, parseObject.StudentNumber,
                        parseObject.CourseName, JsonConvert.SerializeObject(notification)));
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
                _notificationService.SendEmail(_config.GetSection("Notification:Email").Value, string.Format("Olá!, <br/> <br/> Uma entrega falhou a inicar no Diagrama Payment! <br/> <br/> Aqui tem o JSON com a informação {0} <br/><br/> Thanks.", JsonConvert.SerializeObject(notification)));
            }
        }
    }
}