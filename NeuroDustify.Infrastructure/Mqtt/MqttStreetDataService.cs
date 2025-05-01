// NeuroDustify.Infrastructure/Mqtt/MqttStreetDataService.cs
// Provides a concrete implementation of IMqttStreetDataService using the MQTTnet library.
// Handles MQTT connection, subscription, and processing of street data messages.

using MQTTnet; // Core MQTTnet library
using MQTTnet.Client; // MQTTnet client specific classes
using NeuroDustify.Application.Interfaces; // Reference to the Application layer interface
using NeuroDustify.Domain.Entities; // Reference to the Domain layer entities (StreetDataMessage)
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
    /// Concrete implementation of the IMqttStreetDataService using MQTTnet.
    /// Connects to an MQTT broker, subscribes to a street data topic,
    /// and stores received StreetDataMessage objects.
    /// </summary>
    public class MqttStreetDataService : IMqttStreetDataService, IDisposable
    {
        private IMqttClient _mqttClient;
        private MqttClientOptions _mqttClientOptions;
        private readonly ConcurrentBag<StreetDataMessage> _receivedMessages = new ConcurrentBag<StreetDataMessage>();
        private readonly string _brokerAddress;
        private readonly int _port;
        private readonly string _topic;

        public MqttStreetDataService(string brokerAddress, int port, string topic)
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
                .WithClientId($"NeuroDustify_Street_Backend_{Guid.NewGuid()}")
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
                Console.WriteLine($"Attempting to connect MQTT Street client to broker at {_brokerAddress}:{_port}...");
                await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initial MQTT Street Connection failed: {ex.Message}");
            }
        }

        public async Task StopAsync()
        {
            Console.WriteLine("Stopping MQTT Street Data Service...");
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }
            Console.WriteLine("MQTT Street Client stopped.");
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

                    Console.WriteLine($"Subscribed MQTT Street client to topic: {topic}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error subscribing MQTT Street client to topic {topic}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("MQTT Street Client not connected. Cannot subscribe to topic.");
            }
        }

        private void ProcessReceivedMessage(string payload)
        {
            Console.WriteLine($"Attempting to deserialize payload: {payload}");
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var streetData = JsonSerializer.Deserialize<StreetDataMessage>(payload, options);

                if (streetData != null)
                {
                    _receivedMessages.Add(streetData);
                    Console.WriteLine($"Successfully processed and stored Street Data for Street ID: {streetData.StreetId}");
                }
                else
                {
                    Console.WriteLine("Street Data Deserialization returned null. Payload: " + payload);
                }
            }
            catch (JsonException jex)
            {
                Console.WriteLine($"Street Data JSON Deserialization error: {jex.Message}. Payload: {payload}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error processing Street Data message: {ex.Message}. Payload: {payload}");
            }
        }

        public List<StreetDataMessage> GetLatestMessages()
        {
            return _receivedMessages.ToList();
        }

        public void Dispose()
        {
            _mqttClient?.Dispose();
            Console.WriteLine("MqttStreetDataService disposed.");
        }
    }

    public class MqttStreetDataHostedService : IHostedService
    {
        private readonly IMqttStreetDataService _mqttStreetDataService;

        public MqttStreetDataHostedService(IMqttStreetDataService mqttStreetDataService)
        {
            _mqttStreetDataService = mqttStreetDataService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MqttStreetDataHostedService is starting the MQTT Street Data Service.");
            return _mqttStreetDataService.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MqttStreetDataHostedService is stopping the MQTT Street Data Service.");
            return _mqttStreetDataService.StopAsync();
        }
    }
}