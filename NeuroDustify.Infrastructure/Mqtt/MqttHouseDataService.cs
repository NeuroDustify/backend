// NeuroDustify.Infrastructure/Mqtt/MqttHouseDataService.cs
// Provides a concrete implementation of IMqttHouseDataService using the MQTTnet library.
// Handles MQTT connection, subscription, and processing of house data messages.

using MQTTnet; // Core MQTTnet library
using MQTTnet.Client; // MQTTnet client specific classes
using NeuroDustify.Application.Interfaces; // Reference to the Application layer interface
using NeuroDustify.Domain.Entities; // Reference to the Domain layer entities (HouseDataMessage, Location)
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
    /// Concrete implementation of the IMqttHouseDataService using MQTTnet.
    /// Connects to an MQTT broker, subscribes to a house data topic,
    /// and stores received HouseDataMessage objects.
    /// </summary>
    public class MqttHouseDataService : IMqttHouseDataService, IDisposable
    {
        private IMqttClient _mqttClient;
        private MqttClientOptions _mqttClientOptions;
        // Use a thread-safe collection to store received messages
        private readonly ConcurrentBag<HouseDataMessage> _receivedMessages = new ConcurrentBag<HouseDataMessage>();
        private readonly string _brokerAddress;
        private readonly int _port;
        private readonly string _topic;

        /// <summary>
        /// Initializes a new instance of the MqttHouseDataService.
        /// </summary>
        /// <param name="brokerAddress">The address of the MQTT broker.</param>
        /// <param name="port">The port of the MQTT broker.</param>
        /// <param name="topic">The MQTT topic to subscribe to for house data messages.</param>
        public MqttHouseDataService(string brokerAddress, int port, string topic)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(brokerAddress))
                throw new ArgumentException("Broker address cannot be null or whitespace.", nameof(brokerAddress));
            if (port <= 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "Port must be a valid TCP/IP port number.");
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("Topic cannot be null or whitespace.", nameof(topic));


            _brokerAddress = brokerAddress;
            _port = port;
            _topic = topic;

            // Create a new MQTT client instance.
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            // Configure event handlers for MQTT client events
            ConfigureEventHandlers();

            // Configure MQTT client options.
            _mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId($"NeuroDustify_House_Backend_{Guid.NewGuid()}") // Use a unique client ID
                .WithTcpServer(_brokerAddress, _port)
                .WithCleanSession() // Start a fresh session
                                    // .WithCredentials("username", "password") // Uncomment and configure if broker requires authentication
                                    // .WithTls() // Uncomment and configure if connecting via TLS (e.g., port 8883)
                .Build();
        }

        /// <summary>
        /// Configures the event handlers for the MQTT client.
        /// </summary>
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

        /// <summary>
        /// Connects the MQTT client to the broker and starts the internal processing loop.
        /// Implements the StartAsync method from IHostedService (used via MqttHouseDataHostedService).
        /// </summary>
        /// <returns>A Task representing the asynchronous connection operation.</returns>
        public async Task StartAsync()
        {
            // Connect the MQTT client to the broker.
            try
            {
                Console.WriteLine($"Attempting to connect MQTT House client to broker at {_brokerAddress}:{_port}...");
                await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initial MQTT House Connection failed: {ex.Message}");
                // The DisconnectedHandler will attempt to reconnect if the initial connection fails
            }
        }

        /// <summary>
        /// Stops the MQTT client and disconnects from the broker.
        /// Implements the StopAsync method from IHostedService.
        /// </summary>
        /// <returns>A Task representing the asynchronous disconnection operation.</returns>
        public async Task StopAsync()
        {
            Console.WriteLine("Stopping MQTT House Data Service...");
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }
            Console.WriteLine("MQTT House Client stopped.");
        }

        /// <summary>
        /// Subscribes the MQTT client to the configured topic.
        /// </summary>
        /// <param name="topic">The topic to subscribe to.</param>
        private async Task SubscribeToTopic(string topic)
        {
            if (_mqttClient.IsConnected)
            {
                try
                {
                    var subscribeResult = await _mqttClient.SubscribeAsync(
                        new MqttTopicFilterBuilder()
                            .WithTopic(topic)
                            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce) // Request QoS 1
                            .Build());

                    Console.WriteLine($"Subscribed MQTT House client to topic: {topic}");
                    // Optional: Log subscription result details
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error subscribing MQTT House client to topic {topic}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("MQTT House Client not connected. Cannot subscribe to topic.");
            }
        }

        /// <summary>
        /// Processes a received MQTT application message payload (JSON string).
        /// Deserializes the JSON into a HouseDataMessage and stores it.
        /// </summary>
        /// <param name="payload">The JSON payload string received from MQTT.</param>
        private void ProcessReceivedMessage(string payload)
        {
            Console.WriteLine($"Attempting to deserialize payload: {payload}");
            try
            {
                // Deserialize the JSON payload into a HouseDataMessage object
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Console.WriteLine($"Using JsonSerializerOptions: PropertyNameCaseInsensitive = {options.PropertyNameCaseInsensitive}");
                Console.WriteLine($"Target type for deserialization: {typeof(HouseDataMessage).FullName}");

                var houseData = JsonSerializer.Deserialize<HouseDataMessage>(payload, options);

                if (houseData != null)
                {
                    // Store the received message.
                    // In a real application, you'd likely want to handle updates for the same
                    // house ID (e.g., store in a dictionary keyed by PropertyId) rather than
                    // just adding to a bag, depending on whether you need a history or just the latest state.
                    _receivedMessages.Add(houseData);
                    Console.WriteLine($"Successfully processed and stored House Data for Property ID: {houseData.PropertyId}");

                    // Print individual data for debugging
                    Console.WriteLine($"--- Deserialized House Data ---");
                    Console.WriteLine($"Property ID: {houseData.PropertyId}");
                    Console.WriteLine($"Address: {houseData.Address}");
                    // Check if Location is not null before accessing nested properties
                    Console.WriteLine($"Location: {houseData.Location}"); // Prints the type name
                    Console.WriteLine($"Latitude: {(houseData.Location != null ? houseData.Location.Latitude.ToString() : "NULL")}");
                    Console.WriteLine($"Longitude: {(houseData.Location != null ? houseData.Location.Longitude.ToString() : "NULL")}");
                    Console.WriteLine($"Driveways: {houseData.DrivewayIds}"); // DrivewayIds is a string
                    Console.WriteLine($"-------------------------------");
                }
                else
                {
                    Console.WriteLine("House Data Deserialization returned null. Payload: " + payload);
                }
            }
            catch (JsonException jex)
            {
                Console.WriteLine($"House Data JSON Deserialization error: {jex.Message}. Payload: {payload}");
                // Log the type that the deserializer was expecting if possible
                Console.WriteLine($"Expected type: {typeof(HouseDataMessage).FullName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error processing House Data message: {ex.Message}. Payload: {payload}");
            }
        }

        /// <summary>
        /// Retrieves the currently stored latest house data messages.
        /// </summary>
        /// <returns>A list of HouseDataMessage objects.</returns>
        // Note: This returns a snapshot of the data in the ConcurrentBag.
        public List<HouseDataMessage> GetLatestMessages()
        {
            // Convert the ConcurrentBag to a List and return it.
            return _receivedMessages.ToList();
        }

        /// <summary>
        /// Disposes of the MQTT client resources.
        /// Implements the IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            _mqttClient?.Dispose();
            Console.WriteLine("MqttHouseDataService disposed.");
        }
    }

    // --- Hosted Service to manage the MQTT service lifecycle ---
    // This nested class is needed to integrate the MqttHouseDataService into the .NET Core/5+/6+
    // application's background services, ensuring StartAsync and StopAsync are called
    // when the application starts and stops.

    /// <summary>
    /// A hosted service that manages the lifecycle of the MqttHouseDataService.
    /// Ensures the MQTT client connects and disconnects with the application host.
    /// </summary>
    public class MqttHouseDataHostedService : IHostedService
    {
        private readonly IMqttHouseDataService _mqttHouseDataService;

        /// <summary>
        /// Initializes a new instance of the MqttHouseDataHostedService.
        /// </summary>
        /// <param name="mqttHouseDataService">The MQTT house data service instance to manage.</param>
        public MqttHouseDataHostedService(IMqttHouseDataService mqttHouseDataService)
        {
            _mqttHouseDataService = mqttHouseDataService;
        }

        /// <summary>
        /// Called when the application host starts. Starts the MQTT house data service.
        /// </summary>
        /// <param name="cancellationToken">A token to signal cancellation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MqttHouseDataHostedService is starting the MQTT House Data Service.");
            // Call the StartAsync method of the underlying MQTT service
            return _mqttHouseDataService.StartAsync();
        }

        /// <summary>
        /// Called when the application host stops. Stops the MQTT house data service.
        /// </summary>
        /// <param name="cancellationToken">A token to signal cancellation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MqttHouseDataHostedService is stopping the MQTT House Data Service.");
            // Call the StopAsync method of the underlying MQTT service
            return _mqttHouseDataService.StopAsync();
        }
    }
}
