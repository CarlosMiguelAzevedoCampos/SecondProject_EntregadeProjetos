using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace CMA.ISMAI.Delivery.API.CrossCutting.Queue
{
    public class QueueService : IQueueService
    {
        private readonly ILoggingService _log;
        private readonly IConfiguration _config;

        public QueueService(ILoggingService log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }
        public bool SendToQueue(Core.Model.Delivery delivery, string queueName)
        {
            try
            {

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
