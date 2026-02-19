using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Server;
using Newtonsoft.Json;
using ERP.Services.Hubs;
using ERP.BusinessModels.BaseVM;
using ERP.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.API.MQTTConfigurations
{
    public class MQTTService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly MQTTConfig _config;
        private IMqttServer _mqttServer;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private IServiceScopeFactory _serviceScopeFactory;
        public MQTTService(ILogger<MQTTService> logger, IOptions<MQTTConfig> config, IServiceScopeFactory serviceScopeFactory, IHubContext<NotificationHub> notificationHubContext)
        {
            _logger = logger;
            _config = config.Value;
            _notificationHubContext = notificationHubContext;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting MQTT on port " + _config.Port);

            //Building the config
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(1000)
                .WithDefaultEndpointPort(_config.Port);


            //Getting an MQTT Instance
            _mqttServer = new MqttFactory().CreateMqttServer();
            _mqttServer.UseClientConnectedHandler(e =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(e.ClientId) == false)
                    {
                        _logger.LogInformation(e.ClientId + " Connected.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            });
            _mqttServer.UseClientDisconnectedHandler(e =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(e.ClientId) == false)
                    {
                        _logger.LogInformation(e.ClientId + " Disconnected.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            });
            _mqttServer.UseApplicationMessageReceivedHandler(async e =>
            {
                try
                {
                    string topic = e.ApplicationMessage.Topic;
                    if (string.IsNullOrWhiteSpace(topic) == false)
                    {
                        

                    }
                    //var insertedId = await this._mediator.Send(new InsertSensorHistoryCommand());
                    //await _notificationHubContext.Clients.All.SendAsync("sendToUser", model);
                    //string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    //Console.WriteLine($"Topic: {topic}. Message Received: {payload}");
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            });
            //Now, start the server -- Notice this is resturning the MQTT Server's StartAsync, which is a task.
            return _mqttServer.StartAsync(optionsBuilder.Build());
        }


        //private void _mqttServer_ClientUnsubscribedTopic(object sender, MqttServerClientUnsubscribedTopicEventArgs e)
        //{
        //    _logger.LogInformation(e.ClientId + " unsubscribed to " + e.TopicFilter);
        //}

        //private void _mqttServer_ClientSubscribedTopic(object sender, MqttServerClientSubscribedTopicEventArgs e)
        //{
        //    _logger.LogInformation(e.ClientId + " subscribed to " + e.TopicFilter);
        //}

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping MQTT.");
            return _mqttServer.StopAsync();
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");

        }
    }
}
