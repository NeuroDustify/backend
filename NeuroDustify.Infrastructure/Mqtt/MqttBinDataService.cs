// NeuroDustify.Infrastructure/Mqtt/MqttBinDataService.cs
// Provides a concrete implementation of IMqttBinDataService using the MQTTnet library.
// Handles MQTT connection, subscription, and processing of bin data messages.

using MQTTnet;
using MQTTnet.Client;
using NeuroDustify.Application.Interfaces;
using NeuroDustify.Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Hosting;

namespace NeuroDustify.Infrastructure.Mqtt
{
    public class MqttBinDataService : IMqttBinDataService, IDisposable
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttClientOptions;
        private readonly ConcurrentBag<BinDataMessage> _receivedMessages = new();
        private readonly string _brokerAddress;
        private readonly int _port;
        private readonly string _topic;

        public MqttBinDataService(string brokerAddress, int port, string topic)
        {
            if (string.IsNullOrWhiteSpace(brokerAddress)) throw new ArgumentException("Broker address cannot be null or whitespace.", nameof(brokerAddress));
            if (port is <= 0 or > 65535) throw new ArgumentOutOfRangeException(nameof(port), "Port must be a valid TCP/IP port number.");
            if (string.IsNullOrWhiteSpace(topic)) throw new ArgumentException("Topic cannot be null or whitespace.", nameof(topic));

            _brokerAddress = brokerAddress;
            _port = port;
            _topic = topic;

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            ConfigureEventHandlers();

            _mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId($"NeuroDustify_Backend_{Guid.NewGuid()}")
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

            // _mqttClient.ApplicationMessageReceivedAsync += async e =>
            // {
            //     var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.ToArray());
            //     Console.WriteLine($"Message received on topic {e.ApplicationMessage.Topic}: {payload}");
            //     await Task.Run(() => ProcessReceivedMessage(payload));
            // };

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
                Console.WriteLine($"Connecting to MQTT broker at {_brokerAddress}:{_port}...");
                await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }

        public async Task StopAsync()
        {
            Console.WriteLine("Stopping MQTT service...");
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }
            Console.WriteLine("MQTT service stopped.");
        }

        private async Task SubscribeToTopic(string topic)
        {
            if (!_mqttClient.IsConnected)
            {
                Console.WriteLine("Cannot subscribe: MQTT client not connected.");
                return;
            }

            try
            {
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic(topic)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build());

                Console.WriteLine($"Subscribed to topic: {topic}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Subscription failed: {ex.Message}");
            }
        }

        private void ProcessReceivedMessage(string payload)
        {
            try
            {
                var binData = JsonSerializer.Deserialize<BinDataMessage>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (binData != null)
                {
                    _receivedMessages.Add(binData);
                    Console.WriteLine($"Stored Bin Data for Bin ID: {binData.BinId}");
                }
                else
                {
                    Console.WriteLine("Deserialization returned null. Payload: " + payload);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON error: {ex.Message}. Payload: {payload}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}. Payload: {payload}");
            }
        }

        public List<BinDataMessage> GetLatestMessages() => _receivedMessages.ToList();

        public void Dispose()
        {
            _mqttClient?.Dispose();
            Console.WriteLine("MqttBinDataService disposed.");
        }
    }

    public class MqttBinDataHostedService : IHostedService
    {
        private readonly IMqttBinDataService _mqttBinDataService;

        public MqttBinDataHostedService(IMqttBinDataService mqttBinDataService)
        {
            _mqttBinDataService = mqttBinDataService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting MQTT Bin Data Service...");
            return _mqttBinDataService.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping MQTT Bin Data Service...");
            return _mqttBinDataService.StopAsync();
        }
    }
}

