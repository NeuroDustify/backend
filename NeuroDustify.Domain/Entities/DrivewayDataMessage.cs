using System; // Not strictly needed here, but good practice
using System.Collections.Generic; // For List
using System.Text.Json.Serialization; // Required for [JsonPropertyName] attribute

namespace NeuroDustify.Domain.Entities
{
    /// <summary>
    /// Represents the details of a driveway received as a data message via MQTT.
    /// This entity's structure is designed to match the incoming nested JSON payload.
    /// </summary>
    public class DrivewayDataMessage
    {
        /// <summary>
        /// A unique identifier for the driveway. Maps to "driveway_id" in the JSON.
        /// </summary>
        [JsonPropertyName("driveway_id")] // Map JSON key to C# property name
        public required string DrivewayId { get; set; } // Using 'required' for non-nullable property

        /// <summary>
        /// The geographical location of the driveway. Maps to the nested "location" JSON object.
        /// </summary>
        [JsonPropertyName("location")] // Map JSON key to C# property name
        public required Location Location { get; set; } // This property will hold the nested Location object

        /// <summary>
        /// Returns a string representation of the driveway data message.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            return $"DrivewayDataMessage(id={DrivewayId}, lat={Location?.Latitude}, lon={Location?.Longitude})";
        }
    }
}
