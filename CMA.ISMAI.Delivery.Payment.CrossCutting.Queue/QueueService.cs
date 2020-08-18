using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Text;

namespace CMA.ISMAI.Delivery.Payment.CrossCutting.Queue
{
    public class QueueService : IQueueService
    {
        private readonly ILoggingService _log;
        private readonly IConfiguration _config;
        public QueueService(ILoggingService log)
        {
            _log = log;
            _config = new ConfigurationBuilder()
                                                      .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                                                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                      .AddEnvironmentVariables()
                                                      .Build();
        }

        public bool SendToQueue(DeliveryFileSystem deliveryFileSystem, string queue)
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
