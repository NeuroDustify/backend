// NeuroDustify.Infrastructure/Mqtt/MqttDrivewayDataService.cs
// Provides a concrete implementation of IMqttDrivewayDataService using the MQTTnet library.
// Handles MQTT connection, subscription, and processing of driveway data messages.

using MQTTnet; // Core MQTTnet library
using MQTTnet.Client; // MQTTnet client specific classes
using NeuroDustify.Application.Interfaces; // Reference to the Application layer interface
using NeuroDustify.Domain.Entities; // Reference to the Domain layer entities (DrivewayDataMessage)
using System; // For Guid, Console, Exception
using System.Collections.Concurrent; // For ConcurrentBag
using System.Collections.Generic; // For List
using System.Text; // For Encoding
using System.Threading; // For CancellationToken
using System.Threading.Tasks; // For Task
using System.Text.Json; // Using System.Text.Json for deserialization
using System.Linq; // For ToList()
using Microsoft.Extensions.Hosting; // For IHostedService and related types

namespace NeuroDustify.Infrastructure.Mqtt
{
    /// <summary>
    /// Concrete implementation of the IMqttDrivewayDataService using MQTTnet.
    /// Connects to an MQTT broker, subscribes to a driveway data topic,
    /// and stores received DrivewayDataMessage objects.
    /// </summary>
    public class MqttDrivewayDataService : IMqttDrivewayDataService, IDisposable
    {
        private IMqttClient _mqttClient;
        private MqttClientOptions _mqttClientOptions;
        private readonly ConcurrentBag<DrivewayDataMessage> _receivedMessages = new ConcurrentBag<DrivewayDataMessage>();
        private readonly string _brokerAddress;
        private readonly int _port;
        private readonly string _topic;

        public MqttDrivewayDataService(string brokerAddress, int port, string topic)
        {
            if (string.IsNullOrWhiteSpace(brokerAddress))
                throw new ArgumentException("Broker address cannot be null or whitespace.", nameof(brokerAddress));
            if (port <= 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "Port must be a valid TCP/IP port number.");
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("Topic cannot be null or whitespace.", nameof(topic));

            _brokerAddress = brokerAddress;
            _port = port;
            _topic = topic;

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            ConfigureEventHandlers();

            _mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId($"NeuroDustify_Driveway_Backend_{Guid.NewGuid()}")
                .WithTcpServer(_brokerAddress, _port)
                .WithCleanSession()
                .Build();
        }

        private void ConfigureEventHandlers()
        {
            _mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("Connected to MQTT broker.");
                await SubscribeToTopic(_topic);
            };

            _mqttClient.DisconnectedAsync += async e =>
            {
                Console.WriteLine($"Disconnected: {e.ReasonString}. Reconnecting in 5 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Reconnect failed: {ex.Message}");
                }
            };

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                Console.WriteLine($"Message received on topic {e.ApplicationMessage.Topic}: {payload}");
                await Task.Run(() => ProcessReceivedMessage(payload));
            };
        }

        public async Task StartAsync()
        {
            try
            {
                Console.WriteLine($"Attempting to connect MQTT Driveway client to broker at {_brokerAddress}:{_port}...");
                await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initial MQTT Driveway Connection failed: {ex.Message}");
            }
        }

        public async Task StopAsync()
        {
            Console.WriteLine("Stopping MQTT Driveway Data Service...");
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }
            Console.WriteLine("MQTT Driveway Client stopped.");
        }

        private async Task SubscribeToTopic(string topic)
        {
            if (_mqttClient.IsConnected)
            {
                try
                {
                    var subscribeResult = await _mqttClient.SubscribeAsync(
                        new MqttTopicFilterBuilder()
                            .WithTopic(topic)
                            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                            .Build());

                    Console.WriteLine($"Subscribed MQTT Driveway client to topic: {topic}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error subscribing MQTT Driveway client to topic {topic}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("MQTT Driveway Client not connected. Cannot subscribe to topic.");
            }
        }

        private void ProcessReceivedMessage(string payload)
        {
            Console.WriteLine($"Attempting to deserialize payload: {payload}");
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var drivewayData = JsonSerializer.Deserialize<DrivewayDataMessage>(payload, options);

                if (drivewayData != null)
                {
                    _receivedMessages.Add(drivewayData);
                    Console.WriteLine($"Successfully processed and stored Driveway Data for Driveway ID: {drivewayData.DrivewayId}");
                }
                else
                {
                    Console.WriteLine("Driveway Data Deserialization returned null. Payload: " + payload);
                }
            }
            catch (JsonException jex)
            {
                Console.WriteLine($"Driveway Data JSON Deserialization error: {jex.Message}. Payload: {payload}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error processing Driveway Data message: {ex.Message}. Payload: {payload}");
            }
        }

        public List<DrivewayDataMessage> GetLatestMessages()
        {
            return _receivedMessages.ToList();
        }

        public void Dispose()
        {
            _mqttClient?.Dispose();
            Console.WriteLine("MqttDrivewayDataService disposed.");
        }
    }

    public class MqttDrivewayDataHostedService : IHostedService
    {
        private readonly IMqttDrivewayDataService _mqttDrivewayDataService;

        public MqttDrivewayDataHostedService(IMqttDrivewayDataService mqttDrivewayDataService)
        {
            _mqttDrivewayDataService = mqttDrivewayDataService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MqttDrivewayDataHostedService is starting the MQTT Driveway Data Service.");
            return _mqttDrivewayDataService.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MqttDrivewayDataHostedService is stopping the MQTT Driveway Data Service.");
            return _mqttDrivewayDataService.StopAsync();
        }
    }
}