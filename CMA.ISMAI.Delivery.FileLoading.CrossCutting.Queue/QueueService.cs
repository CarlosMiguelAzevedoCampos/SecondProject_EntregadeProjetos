using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.Queue
{
    public class QueueService : IQueueService
    {
        private readonly ILoggingService _log;

        public QueueService(ILoggingService log)
        {
            _log = log;
        }

        public bool SendToQueue(DeliveryFileSystem deliveryFileSystem, string queue)
        {
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
                    channel.QueueDeclare(queue: queue,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deliveryFileSystem, settings));

                    channel.BasicPublish(exchange: "",
                                         routingKey: queue,
                                         basicProperties: null,
                                         body: body);
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }
    }
}
