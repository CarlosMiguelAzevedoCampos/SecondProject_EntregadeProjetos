using CMA.ISMAI.Delivery.EventStore.Interface;
using CMA.ISMAI.Delivery.Logging.Interface;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using NetDevPack.Messaging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace CMA.ISMAI.Delivery.EventStore.Service
{
    public class EventStoreService : IEventStoreService
    {

        private IEventStoreConnection _connection = null;
        private readonly ILoggingService _log;
        private readonly IConfiguration _config;
        public EventStoreService(ILoggingService log)
        {
            _log = log;
            _config = new ConfigurationBuilder()
                                                      .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                                                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                      .AddEnvironmentVariables()
                                                      .Build();
        }

        public void SaveToEventStore(Event @event)
        {
            try
            {
                BuildConnection();
                _connection.AppendToStreamAsync(
                         @event.GetType().FullName,
                        ExpectedVersion.Any,
                        new EventData(
                            Guid.NewGuid(),
                            @event.GetType().FullName,
                            false,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                            new byte[] { }
                        )
                    ).Wait();
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
        }

        private void BuildConnection()
        {
            if (_connection != null)
                return;
            _connection = EventStoreConnection.Create(new Uri(_config.GetSection("EventStore:IPAddress").Value));
            _connection.ConnectAsync().Wait();
        }
    }
}