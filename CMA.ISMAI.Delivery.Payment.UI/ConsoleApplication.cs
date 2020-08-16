using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda.Interface;
using NetDevPack.Mediator;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace CMA.ISMAI.Delivery.Payment.UI
{
    internal class ConsoleApplication
    {
        private readonly ICamundaService _camundaService;
        private readonly ILoggingService _log;

        public ConsoleApplication(ICamundaService camundaService, ILoggingService log)
        {
            _camundaService = camundaService;
            _log = log;
        }

        public void StartService()
        {
            try
            {
                _log.Info($"Payment is starting.. {DateTime.Now}");
                _camundaService.RegistWorkers();
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    Port = 5672,
                    UserName = "admin",
                    Password = "admin"
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


                    Console.ReadKey();
                }
                Console.ReadKey();

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}, RabbitMQ starting..? ", ex.ToString()));
                Console.WriteLine("Retrying in 30 seconds..");
                _log.Fatal(string.Format("{0}, RabbitMQ starting..? ", ex.ToString()));
                Thread.Sleep(30000);
                StartService();
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
            _camundaService.StartWorkFlow((Core.Model.DeliveryFileSystem)notification);
        }
    }
}