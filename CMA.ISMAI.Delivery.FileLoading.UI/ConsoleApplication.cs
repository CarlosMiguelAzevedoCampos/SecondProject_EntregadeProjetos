using CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda.Interface;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using NetDevPack.Mediator;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace CMA.ISMAI.Delivery.FileLoading.UI
{
    internal class ConsoleApplication
    {
        private readonly IMediatorHandler _mediatr;
        private readonly ICamundaService _ca;

        public ConsoleApplication(IMediatorHandler mediatr, ICamundaService ca)
        {
            _mediatr = mediatr;
            _ca = ca;
        }

        public void StartService()
        {
            _ca.RegistWorkers();
            try
            {
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
                    channel.QueueDeclare(queue: "FileLoading",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;
                    channel.BasicConsume(queue: "FileLoading",
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
            // _ca.StartWorkFlow(new Core.Model.DeliveryWithLink(Guid.NewGuid(), "Carlos", "ISMAI", "Informática", "a029216@ismai.pt", "a029216", DateTime.Now, "", "José", "Mestrado", "Jose", "safa", "safas"));
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var notification = JsonConvert.DeserializeObject
                    (Encoding.UTF8.GetString(e.Body.ToArray()), settings);
            _ca.StartWorkFlow((Core.Model.Delivery)notification);
        }
    }
}