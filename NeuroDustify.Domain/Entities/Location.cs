// NeuroDustify.Domain/Entities/Location.cs
// Represents a geographical location with latitude and longitude.
// This is a Value Object in the Domain layer.

namespace NeuroDustify.Domain.Entities
{
    /// <summary>
    /// Represents a geographical location with latitude and longitude coordinates.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// The latitude coordinate.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude coordinate.
        /// </summary>
        public double Longitude { get; set; }

        // Optional: Add constructors, equality checks, or other methods if needed
        // For a simple data model, properties might be sufficient.
    }
}
