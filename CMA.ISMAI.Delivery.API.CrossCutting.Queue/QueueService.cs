using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace CMA.ISMAI.Delivery.API.CrossCutting.Queue
{
    public class QueueService : IQueueService
    {
        private readonly ILoggingService _log;

        public QueueService(ILoggingService log)
        {
            _log = log;
        }
        public bool SendToQueue(Core.Model.Delivery delivery, string queueName)
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
                    channel.QueueDeclare(queue: queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(delivery, settings));

                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
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
