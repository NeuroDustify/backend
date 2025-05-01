// NeuroDustify.Domain/Entities/StreetDataMessage.cs
// Represents the structure of the house data received via MQTT,
// adjusted to correctly deserialize the nested JSON payload structure.

using System; // Not strictly needed here, but good practice
using System.Collections.Generic; // For List
using System.Text.Json.Serialization; // Required for [JsonPropertyName] attribute

namespace NeuroDustify.Domain.Entities
{
    /// <summary>
    /// Represents the details of a street received as a data message via MQTT.
    /// This entity's structure is designed to match the incoming nested JSON payload.
    /// </summary>
    public class StreetDataMessage
    {
        /// <summary>
        /// A unique identifier for the street. Maps to "street_id" in the JSON.
        /// </summary>
        [JsonPropertyName("street_id")] // Map JSON key to C# property name
        public required string StreetId { get; set; } // Using 'required' for non-nullable property

        /// <summary>
        /// The street address. Maps to "address" in the JSON.
        /// </summary>
        [JsonPropertyName("name")] // Map JSON key to C# property name
        public required string Name { get; set; } // Using 'required' for non-nullable property

        /// <summary>
        /// The geographical location of the street. Maps to the nested "location" JSON object.
        /// </summary>
        [JsonPropertyName("house_ids")] // Map JSON key to C# property name
        public required string HouseId { get; set; } // This property will hold the nested Location object

        // Optional: Add constructors, equality checks, or other methods if needed
    }
}