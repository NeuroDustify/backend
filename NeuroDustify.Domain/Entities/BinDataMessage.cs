// NeuroDustify.Domain/Entities/BinDataMessage.cs
// Represents the core entity for a single data message received from a smart bin.
// This is an Entity in the Domain layer, modeling the structure of the MQTT payload.

using System; // Required for DateTime

namespace NeuroDustify.Domain.Entities
{
    /// Represents a single data message received from a smart bin sensor.
    /// This is a core entity in the Domain model, capturing the state of a bin
    /// at a specific point in time.
    /// </summary>
    public class BinDataMessage
    {
        /// <summary>
        /// The unique identifier of the smart bin that sent the message.
        /// </summary>
        public required string BinId { get; set; }

        /// <summary>
        /// The timestamp when the data reading was taken by the bin sensor (in UTC).
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The specific geographical location of the bin when the data was recorded.
        /// This might be slightly different from the house location if the bin is
        /// placed at the curb.
        /// </summary>
        // Uses the Location value object defined above
        public required Location Location { get; set; }

        /// <summary>
        /// The fill level of the bin, typically expressed as a percentage (0-100).
        /// </summary>
        public double FillLevelPercentage { get; set; }

        /// <summary>
        /// The operational status of the bin at the time of the reading
        /// (e.g., "online", "low battery", "error", "maintenance").
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// The temperature inside the bin, measured in degrees Celsius.
        /// Useful for detecting potential issues like fires.
        /// </summary>
        public double TemperatureCelsius { get; set; }

        /// <summary>
        /// Details about the house associated with this bin's location.
        /// This provides context for where the bin is located.
        /// </summary>
        // Uses the AssociatedHouse entity/value object defined above
        public required AssociatedHouse AssociatedHouse { get; set; }

        // Optional: Add methods here representing business logic related to a bin data message,
        // e.g., IsFull(), IsOverheating(), CalculateAge(), ValidateData()
        // public bool IsFull() => FillLevelPercentage >= 90;
        // public bool IsOverheating(double threshold) => TemperatureCelsius > threshold;
    }
}
