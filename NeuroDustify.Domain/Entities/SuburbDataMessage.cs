using System;
using System.Collections.Generic;

namespace NeuroDustify.Domain.Entities
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class DrivewayMessage
    {
        public string Id { get; set; } // Changed to string? to match the python data
        public Location Location { get; set; }
    }

    public class HouseMessage
    {
        public string PropertyId { get; set; }
        public string Address { get; set; }
        public Location Location { get; set; }
    }

    public class StreetMessage
    {
        public string StreetId { get; set; }
        public string Name { get; set; }
        public List<HouseMessage> Houses { get; set; }
    }

    public class SuburbMessage
    {
        public string SuburbId { get; set; }
        public string Name { get; set; }
        public List<StreetMessage> Streets { get; set; }
    }

        public class SuburbDataMessage
    {
        public string SuburbId { get; set; } // Example: "suburb_123"
        public string Name { get; set; }
        public List<StreetData>? Streets { get; set; }  // assuming you have a StreetData class
        // Add other properties as needed
    }

    public class StreetData
    {
        public string StreetId { get; set; }
        public string Name { get; set; }
        public List<HouseData>? Houses { get; set; }
    }

    public class HouseData
    {
         public string HouseId { get; set; }
         public string Address { get; set; }
         public Location HouseLocation { get; set; }
    }
}
