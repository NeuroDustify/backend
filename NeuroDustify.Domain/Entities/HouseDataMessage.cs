// NeuroDustify.Domain/Entities/HouseDataMessage.cs
// Represents the structure of the house data received via MQTT,
// adjusted to correctly deserialize the nested JSON payload structure.

using System; // Not strictly needed here, but good practice
using System.Collections.Generic; // For List
using System.Text.Json.Serialization; // Required for [JsonPropertyName] attribute

namespace NeuroDustify.Domain.Entities
{
    /// <summary>
    /// Represents a geographical location with latitude and longitude coordinates,
    /// as nested within the incoming HouseDataMessage JSON.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// The latitude coordinate. Maps to "latitude" within the "location" JSON object.
        /// </summary>
        [JsonPropertyName("latitude")] // Map JSON key to C# property name
        public required double Latitude { get; set; } // Using 'required' for non-nullable property

        /// <summary>
        /// The longitude coordinate. Maps to "longitude" within the "location" JSON object.
        /// </summary>
        [JsonPropertyName("longitude")] // Map JSON key to C# property name
        public required double Longitude { get; set; } // Using 'required' for non-nullable property

        // Optional: Add constructors, equality checks, or other methods if needed
    }

    /// <summary>
    /// Represents the details of a house received as a data message via MQTT.
    /// This entity's structure is designed to match the incoming nested JSON payload.
    /// </summary>
    public class HouseDataMessage
    {
        /// <summary>
        /// A unique identifier for the property/house. Maps to "property_id" in the JSON.
        /// </summary>
        [JsonPropertyName("property_id")] // Map JSON key to C# property name
        public required string PropertyId { get; set; } // Using 'required' for non-nullable property

        /// <summary>
        /// The street address of the house. Maps to "address" in the JSON.
        /// </summary>
        [JsonPropertyName("address")] // Map JSON key to C# property name
        public required string Address { get; set; } // Using 'required' for non-nullable property

        /// <summary>
        /// The geographical location of the house. Maps to the nested "location" JSON object.
        /// </summary>
        [JsonPropertyName("location")] // Map JSON key to C# property name
        public required Location Location { get; set; } // This property will hold the nested Location object

        /// <summary>
        /// A list of Driveway Ids associated with the house (as a comma-separated string, or similar, based on publisher).
        /// Maps to "driveway_ids" in the JSON.
        /// </summary>
        [JsonPropertyName("driveway_ids")] // Map JSON key to C# property name
        public required string DrivewayIds { get; set; } // Using 'required' for non-nullable property


        // Note: Removed the explicit constructor. With 'required' properties and
        // JSON deserialization, object initializers are implicitly used.

        /// <summary>
        /// Returns a string representation of the house data message.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            // Updated ToString to reflect accessing data through the nested Location object
            return $"HouseDataMessage(id={PropertyId}, address='{Address}', lat={Location?.Latitude}, lon={Location?.Longitude}, driveways='{DrivewayIds}')";
        }
    }
}
