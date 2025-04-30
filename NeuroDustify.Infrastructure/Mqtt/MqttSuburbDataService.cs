// using MQTTnet;
// using MQTTnet.Client;
// using NeuroDustify.Application.Interfaces;
// using NeuroDustify.Domain.Entities;
// using System;
// using System.Collections.Concurrent;
// using System.Collections.Generic;
// using System.Text;
// using System.Text.Json;
// using System.Threading;
// using System.Threading.Tasks;
// using System.Linq;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging; // Added for logging


// namespace NeuroDustify.Infrastructure.Mqtt
// {
//     public class MqttSuburbDataService : IMqttSuburbDataService, IDisposable
//     {
//         private readonly IMqttClient _mqttClient;
//         private readonly MqttClientOptions _mqttClientOptions;
//         private readonly ConcurrentBag<SuburbDataMessage> _receivedMessages = new();
//         private readonly string _brokerAddress;
//         private readonly int _port;
//         private readonly string _topic;
//         private readonly ILogger<MqttSuburbDataService> _logger; // Add logger


//         public MqttSuburbDataService(string brokerAddress, int port, string topic, ILogger<MqttSuburbDataService> logger) // Added ILogger
//         {
//             if (string.IsNullOrWhiteSpace(brokerAddress)) throw new ArgumentException("Broker address cannot be null or whitespace.", nameof(brokerAddress));
//             if (port is <= 0 or > 65535) throw new ArgumentOutOfRangeException(nameof(port), "Port must be a valid TCP/IP port number.");
//             if (string.IsNullOrWhiteSpace(topic)) throw new ArgumentException("Topic cannot be null or whitespace.", nameof(topic));

//             _brokerAddress = brokerAddress;
//             _port = port;
//             _topic = topic;
//             _logger = logger; // Store the logger
//             var factory = new MqttFactory();
//             _mqttClient = factory.CreateMqttClient();

//             _mqttClientOptions = new MqttClientOptionsBuilder()
//                 .WithTcpServer(_brokerAddress, _port)
//                 .WithCleanSession() // Start with a clean session
//                 .Build();

//             _mqttClient.ApplicationMessageReceivedAsync += HandleMessageReceivedAsync;
//             _mqttClient.DisconnectedAsync += HandleDisconnectedAsync; //handle disconnect
//         }

//         private Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs e)
//         {
//             _logger.LogWarning($"Disconnected from MQTT Broker. Reason: {e.ReasonString}.");
//             return Task.CompletedTask;
//         }


//         public async Task StartAsync()
//         {
//             try
//             {
//                 await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
//                 _logger.LogInformation($"Connected to MQTT Broker: {_brokerAddress}:{_port}");

//                 var mqttSubscribeOptions = new MqttSubscribeOptionsBuilder()
//                     .WithTopicFilter(f => f.WithTopic(_topic))
//                     .Build();

//                 await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
//                 _logger.LogInformation($"Subscribed to topic: {_topic}");
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error starting MQTT service.");
//                 throw; // Re-throw the exception to prevent silent failure.
//             }
//         }

//         public async Task StopAsync()
//         {
//             if (_mqttClient == null)
//                 return;

//             try
//             {
//                 await _mqttClient.DisconnectAsync(MqttClientDisconnectOptions.Normal, CancellationToken.None);
//                 _logger.LogInformation("Disconnected from MQTT Broker.");
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error stopping MQTT service.");
//                 // Optionally, you might not want to re-throw here, as stopping
//                 // the service might be part of a larger shutdown sequence.
//             }
//         }

//         private Task HandleMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
//         {
//             try
//             {
//                 string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.ToArray());
//                 _logger.LogInformation($"Received message on topic: {e.ApplicationMessage.Topic}");
//                 //_logger.LogInformation($"Payload: {payload}"); // Remove this line to reduce log verbosity

//                 // Attempt to deserialize the payload
//                 SuburbDataMessage? suburbDataMessage = JsonSerializer.Deserialize<SuburbDataMessage>(payload); //, _jsonSerializerOptions);  -- removed options

//                 if (suburbDataMessage != null)
//                 {
//                     _receivedMessages.Add(suburbDataMessage);
//                     _logger.LogDebug($"Processed Suburb Data for Suburb ID: {suburbDataMessage.SuburbId}"); // changed from .BinId to .SuburbId
//                 }
//                 else
//                 {
//                     _logger.LogWarning("Deserialization returned null. Payload: " + payload);
//                 }
//             }
//             catch (JsonException ex)
//             {
//                 _logger.LogError(ex, $"JSON error: {ex.Message}. Payload: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.ToArray())}");
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, $"Error processing message: {ex.Message}. Payload: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.ToArray())}");
//             }
//             return Task.CompletedTask;
//         }

//         public List<SuburbDataMessage> GetLatestMessages() => _receivedMessages.ToList();

//         public void Dispose()
//         {
//             _mqttClient?.Dispose();
//             _logger.LogInformation("MqttSuburbDataService disposed.");
//         }

//         public SuburbDataMessage? GetLatestSuburbData()
//         {
//             return _receivedMessages.LastOrDefault();
//         } 

//     }



//     public class MqttSuburbDataHostedService : IHostedService
//     {
//         private readonly IMqttSuburbDataService _mqttSuburbDataService;
//         private readonly ILogger<MqttSuburbDataHostedService> _logger; // Add logger



//         public MqttSuburbDataHostedService(IMqttSuburbDataService mqttSuburbDataService, ILogger<MqttSuburbDataHostedService> logger) // Added logger
//         {
//             _mqttSuburbDataService = mqttSuburbDataService;
//             _logger = logger; // Store the logger
//         }

//         public Task StartAsync(CancellationToken cancellationToken)
//         {
//             _logger.LogInformation("Starting MQTT Suburb Data Service...");
//             return _mqttSuburbDataService.StartAsync();
//         }

//         public Task StopAsync(CancellationToken cancellationToken)
//         {
//             _logger.LogInformation("Stopping MQTT Suburb Data Service...");
//             return _mqttSuburbDataService.StopAsync();
//         }
//     }
// }

