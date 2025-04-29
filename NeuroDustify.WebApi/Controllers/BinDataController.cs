// NeuroDustify.WebApi/Controllers/BinDataController.cs
// Web API controller for accessing bin data received via MQTT.
// This controller belongs in the WebApi (Presentation) layer.

using Microsoft.AspNetCore.Mvc; // Base classes for MVC/API controllers
using NeuroDustify.Application.Interfaces; // Reference to the Application layer interface
using NeuroDustify.Domain.Entities; // Reference to the Domain layer entities
using System.Collections.Generic; // For List
using System.Linq; // For LINQ methods like Where, FirstOrDefault
using System; // For Console

namespace NeuroDustify.WebApi.Controllers
{
    [ApiController] // Indicates that this class is an API controller
    [Route("api/[controller]")] // Defines the base route for this controller (e.g., /api/BinData)
    public class BinDataController : ControllerBase // Inherits from ControllerBase for API functionality
    {
        // Dependency Injection: The controller depends on the IMqttBinDataService interface.
        // The actual implementation (MqttBinDataService) will be provided at runtime by the DI container.
        private readonly IMqttBinDataService _mqttBinDataService;

        /// <summary>
        /// Initializes a new instance of the BinDataController.
        /// </summary>
        /// <param name="mqttBinDataService">The injected MQTT bin data service.</param>
        public BinDataController(IMqttBinDataService mqttBinDataService)
        {
            _mqttBinDataService = mqttBinDataService;
        }

        /// <summary>
        /// Retrieves the latest bin data messages received via MQTT.
        /// </summary>
        /// <returns>An ActionResult containing a list of BinDataMessage objects or a NotFound result.</returns>
        [HttpGet("latest")] // Defines a GET endpoint at /api/BinData/latest
        public ActionResult<List<BinDataMessage>> GetLatestBinData()
        {
            Console.WriteLine("Received request for latest bin data.");
            // Call the method on the injected service to get all received data
            var latestData = _mqttBinDataService.GetLatestMessages();

            // Check if any data was received
            if (latestData == null || latestData.Count == 0)
            {
                // Return a 404 Not Found response if no data is available
                Console.WriteLine("No bin data messages received yet. Returning 404.");
                return NotFound("No bin data messages received yet.");
            }

            // Return a 200 OK response with the list of bin data messages
            Console.WriteLine($"Returning {latestData.Count} latest bin data messages.");
            return Ok(latestData);
        }

        /// <summary>
        /// Retrieves the latest data message for a specific bin ID.
        /// </summary>
        /// <param name="binId">The unique identifier of the bin.</param>
        /// <returns>An ActionResult containing the latest BinDataMessage for the specified bin, or a NotFound result.</returns>
        [HttpGet("{binId}")] // Defines a GET endpoint at /api/BinData/{binId}
        public ActionResult<BinDataMessage> GetBinDataById(string binId)
        {
            Console.WriteLine($"Received request for latest bin data for Bin ID: {binId}");

            // Retrieve all received messages from the service
            var allMessages = _mqttBinDataService.GetLatestMessages();

            // Find the latest message for the specified bin ID
            // Note: This assumes the latest message in the list is the most recent for that bin.
            // A more robust implementation would sort by timestamp or query a database.
            var latestBinData = allMessages
                                .Where(m => m.BinId.Equals(binId, StringComparison.OrdinalIgnoreCase)) // Filter by Bin ID (case-insensitive)
                                .OrderByDescending(m => m.Timestamp) // Order by timestamp descending
                                .FirstOrDefault(); // Get the first (latest) one, or null if none found

            // Check if data for the specific bin was found
            if (latestBinData == null)
            {
                // Return a 404 Not Found response if no data for this bin ID is available
                Console.WriteLine($"No bin data found for Bin ID: {binId}. Returning 404.");
                return NotFound($"No bin data found for Bin ID: {binId}.");
            }

            // Return a 200 OK response with the latest bin data message
            Console.WriteLine($"Returning latest bin data for Bin ID: {binId}.");
            return Ok(latestBinData);
        }

        // You could add other endpoints here, e.g., to get all data for a bin,
        // filter by time range, etc.
        // [HttpGet("{binId}/all")]
        // public ActionResult<List<BinDataMessage>> GetAllBinDataById(string binId) { ... }

        // [HttpGet("by-time")]
        // public ActionResult<List<BinDataMessage>> GetBinDataByTimeRange([FromQuery] DateTime startTime, [FromQuery] DateTime endTime) { ... }
        // Note: Implementing time range filtering would likely require updating IMqttBinDataService
        // and its implementation to support querying by time.
    }
}
