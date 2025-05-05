// NeuroDustify.Domain/Entities/AssociatedHouse.cs
// Represents the associated house details included in the bin data message.
// This is an Entity or Value Object in the Domain layer, depending on its identity.

namespace NeuroDustify.Domain.Entities
{
    /// <summary>
    /// Represents the details of the house associated with a smart bin.
    /// </summary>
    public class AssociatedHouse
    {
        /// <summary>
        /// The unique identifier for the property/house.
        /// </summary>
        public required string PropertyId { get; set; }

        /// <summary>
        /// The street address of the house.
        /// </summary>
        public required string Address { get; set; }

        /// <summary>
        /// The geographical location of the house.
        /// </summary>
        // Reusing the Location class from the same Domain layer
        public required Location HouseLocation { get; set; }

        // Optional: Add constructors or other methods if needed
    }
}
