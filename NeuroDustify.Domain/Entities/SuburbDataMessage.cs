// NeuroDustify.Domain/Entities/SuburbDataMessage.cs
// Represents the structure of the suburb data received via MQTT,
// adjusted to correctly deserialize the nested JSON payload structure.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization; // Required for [JsonPropertyName] attribute

namespace NeuroDustify.Domain.Entities
{
    // Keeping other message types for now, but the primary one for the suburb topic is SuburbDataMessage
    // public class DrivewayMessage
    // {
    //     [JsonPropertyName("id")] // Assuming JSON key is "id" or "driveway_id", check your CSV/JSON
    //     public string? Id { get; set; }
    //     [JsonPropertyName("location")]
    //     public Location? Location { get; set; } // Make nullable if location might be missing
    // }

    // public class HouseMessage
    // {
    //     [JsonPropertyName("property_id")] // Assuming JSON key, check your CSV/JSON
    //     public string? PropertyId { get; set; }
    //     [JsonPropertyName("address")] // Assuming JSON key
    //     public string? Address { get; set; }
    //     [JsonPropertyName("location")] // Assuming JSON key
    //     public Location? Location { get; set; } // Make nullable if location might be missing
    // }

    //  public class StreetMessage // Assuming this structure is published separately or nested differently
    // {
    //     [JsonPropertyName("street_id")] // Assuming JSON key
    //     public string? StreetId { get; set; }
    //     [JsonPropertyName("name")] // Assuming JSON key
    //     public string? Name { get; set; }
    //     // Assuming Houses are not directly nested in the street message based on the publisher sending them separately
    //     // public List<HouseMessage>? Houses { get; set; }
    // }

    // // This SuburbMessage structure might correspond to a different data feed or model
    // public class SuburbMessage
    // {
    //     [JsonPropertyName("suburb_id")] // Assuming JSON key
    //     public string? SuburbId { get; set; }
    //     [JsonPropertyName("name")] // Assuming JSON key
    //     public string? Name { get; set; }
    //     // Assuming Streets are not directly nested in this message based on the publisher sending them separately
    //     // public List<StreetMessage>? Streets { get; set; }
    // }


    /// <summary>
    /// Represents the details of a suburb received as a data message via MQTT,
    /// specifically tailored to match the top-level JSON structure published to the 'suburb' topic.
    /// </summary>
    public class SuburbDataMessage
    {
        /// <summary>
        /// A unique identifier for the suburb. Maps to "suburb_id" in the JSON.
        /// </summary>
        [JsonPropertyName("suburb_id")]
        public string? SuburbId { get; set; } // Made nullable

        /// <summary>
        /// The suburb name. Maps to "name" in the JSON.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; } // Made nullable


        /// <summary>
        /// A property potentially holding street identifiers. Maps to "street_ids" in the JSON.
        /// Keeping as string? based on your original model, assuming it's a single ID or a string of IDs.
        /// If it's a JSON array of strings, change this to List<string>?.
        /// </summary>
        [JsonPropertyName("street_ids")]
        public string? StreetId { get; set; } // Made nullable

    }

    // Keeping the StreetData and HouseData definitions from the other SuburbDataMessage
    // definition in your original file, in case they are used elsewhere or are intended
    // for a different message structure.

    //  public class StreetData // Assuming this structure is published separately or nested differently
    // {
    //     [JsonPropertyName("street_id")] // Assuming JSON key
    //     public string? StreetId { get; set; }
    //     [JsonPropertyName("name")] // Assuming JSON key
    //     public string? Name { get; set; }
    //     [JsonPropertyName("houses")] // Assuming JSON key for nested houses
    //     public List<HouseData>? Houses { get; set; } // Made nullable
    // }

    // public class HouseData // Assuming this structure is published separately or nested differently
    // {
    //      [JsonPropertyName("house_id")] // Assuming JSON key
    //      public string? HouseId { get; set; }
    //      [JsonPropertyName("address")] // Assuming JSON key
    //      public string? Address { get; set; }
    //      [JsonPropertyName("location")] // Assuming JSON key
    //      public Location? HouseLocation { get; set; } // Made nullable
    // }
}