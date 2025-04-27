using System;
using System.Text;
using MQTTnet;
using Newtonsoft.Json;

namespace SmartDustbin.MqttService
{
    public class MqttSubscriberService : IDisposable
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttOptions;
        private readonly string _mqttBrokerAddress = "test.mosquitto.org";
        private readonly int _mqttBrokerPort = 1883;
        private readonly string _mqttTopic = "suburb/model/igention/#";

        public MqttSubscriberService()
        {
            var factory = new MqttClientFactory();
            _mqttClient = factory.CreateMqttClient();

            _mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_mqttBrokerAddress, _mqttBrokerPort)
                .Build();

            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                return HandleMessageReceivedAsync(e);
            };
            _mqttClient.DisconnectedAsync += e =>
            {
                Console.WriteLine($"Disconnected from MQTT Broker. Reason: {e.ReasonString}. Trying to reconnect...");
                return Task.CompletedTask;
            };
        }

        public async Task StartAsync()
        {
            try
            {
                await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
                Console.WriteLine($"Connected to MQTT Broker: {_mqttBrokerAddress}:{_mqttBrokerPort}");

                var mqttSubscribeOptions = new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(f => { f.WithTopic(_mqttTopic); })
                    .Build();

                await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
                Console.WriteLine($"Subscribed to topic: {_mqttTopic}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting or subscribing: {ex.Message}");
            }
        }

        public async Task StopAsync()
        {
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync(new MqttClientDisconnectOptions(), CancellationToken.None);
                Console.WriteLine("Disconnected from MQTT Broker");
            }
        }

        public void Dispose()
        {
            _mqttClient.Dispose();
        }

        private async Task HandleMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine($"Received message on topic: {e.ApplicationMessage.Topic}");
            Console.WriteLine($"Payload: {payload}");

            try
            {
                // Check the topic to determine how to deserialize and process the message
                if (e.ApplicationMessage.Topic.Contains("bins"))
                {
                    // Deserialize the payload into a BinData object
                    BinData? binData = JsonConvert.DeserializeObject<BinData>(payload); // Made BinData nullable

                    // Process the bin data
                    if (binData != null) // Null check before accessing properties
                    {
                        Console.WriteLine("Bin Data:");
                        Console.WriteLine($"   Bin ID: {binData.bin_id}");
                        Console.WriteLine($"   Fill Level: {binData.fill_level_percentage}");
                        Console.WriteLine($"   Temperature: {binData.temperature_celsius}");
                        Console.WriteLine($"   Status: {binData.status}");
                        Console.WriteLine("   House:");
                        if (binData.house != null)
                        {
                            Console.WriteLine($"      House ID: {binData.house.house_id}");
                            Console.WriteLine($"      Address: {binData.house.address}");
                            if (binData.house.location != null)
                            {
                                Console.WriteLine($"      Location: Latitude={binData.house.location.latitude}, Longitude={binData.house.location.longitude}");
                            }
                        }
                    }
                    else
                    {
                         Console.WriteLine("   Error: Bin data is null.");
                    }
                }
                else if (e.ApplicationMessage.Topic.Contains("driveways"))
                {
                    var drivewayData = JsonConvert.DeserializeObject<DrivewayData>(payload);
                    Console.WriteLine($"   Driveway ID: {drivewayData?.id}, Location: {drivewayData?.location?.latitude}, {drivewayData?.location?.longitude}");
                }
                else if (e.ApplicationMessage.Topic.Contains("houses"))
                {
                    // Deserialize into HouseData (you'll need to create this class)
                    // var houseData = JsonConvert.DeserializeObject<HouseData>(payload);
                    // Console.WriteLine($"   House Info: ...");
                }
                else if (e.ApplicationMessage.Topic.Contains("streets"))
                {
                    // Deserialize into StreetData
                }
                else if (e.ApplicationMessage.Topic.Contains("suburb"))
                {
                    // Deserialize into SuburbData
                }
                else
                {
                    Console.WriteLine("   Unknown data type.");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"   Error deserializing JSON: {ex.Message}. Payload was: {payload}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error processing message: {ex.Message}");
            }
        }
    }

    // Define the C# classes to match the JSON structure of the bin data
    public class BinData
    {
        public required string bin_id { get; set; }  // Use required
        public double fill_level_percentage { get; set; }
        public float temperature_celsius { get; set; }
        public required string status { get; set; } // Use required
        public required HouseData house { get; set; } // Nested House object, Use required
    }

    public class HouseData
    {
        public required string house_id { get; set; } // Use required
        public required string address { get; set; } // Use required
        public required Location location { get; set; }  // Nested Location object, Use required
    }

    public class Location
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
      public class DrivewayData
    {
        public string? id { get; set; }
        public Location? location { get; set; }
    }
}

