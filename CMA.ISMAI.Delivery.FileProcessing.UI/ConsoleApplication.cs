using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Camunda.Interface;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace CMA.ISMAI.Delivery.FileProcessing.UI
{
    internal class ConsoleApplication
    {
        private readonly ICamundaService _camundaService;

        public ConsoleApplication(ICamundaService camundaService)
        {
            _camundaService = camundaService;
        }

        public void StartService()
        {
            try
            {
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
                    channel.QueueDeclare(queue: "FileProcessing",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;
                    channel.BasicConsume(queue: "FileProcessing",
                         autoAck: true,
                         consumer: consumer);
                    Console.ReadKey();
                }
                Console.ReadKey();

            }
            catch (Exception ex)
            {
                //Console.WriteLine(string.Format("{0}, RabbitMQ starting..? ", ex.ToString()));
                //Console.WriteLine("Retrying in 30 seconds..");
                //serviceProvider.GetService<ILog>().Fatal(string.Format("{0}, RabbitMQ starting..? ", ex.ToString()));
                //Thread.Sleep(30000);
                //await Main(args);
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