using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace CMA.ISMAI.Delivery.Payment.CrossCutting.Queue
{
    public class QueueService : IQueueService
    {

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
                // _log.Fatal(ex.ToString());
            }
            return false;
        }
    }
}
