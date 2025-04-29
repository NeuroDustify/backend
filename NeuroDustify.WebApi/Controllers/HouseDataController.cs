// NeuroDustify.WebApi/Controllers/HouseDataController.cs
// Web API controller for accessing house data received via MQTT.
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
    [Route("api/[controller]")] // Defines the base route for this controller (e.g., /api/HouseData)
    public class HouseDataController : ControllerBase // Inherits from ControllerBase for API functionality
    {
        // Dependency Injection: The controller depends on the IMqttHouseDataService interface.
        // The actual implementation (MqttHouseDataService) will be provided at runtime by the DI container.
        private readonly IMqttHouseDataService _mqttHouseDataService;

        /// <summary>
        /// Initializes a new instance of the HouseDataController.
        /// </summary>
        /// <param name="mqttHouseDataService">The injected MQTT house data service.</param>
        public HouseDataController(IMqttHouseDataService mqttHouseDataService)
        {
            _mqttHouseDataService = mqttHouseDataService;
        }

        /// <summary>
        /// Retrieves the latest house data messages received via MQTT.
        /// </summary>
        /// <returns>An ActionResult containing a list of HouseDataMessage objects or a NotFound result.</returns>
        [HttpGet("latest")] // Defines a GET endpoint at /api/HouseData/latest
        public ActionResult<List<HouseDataMessage>> GetLatestHouseData()
        {
            Console.WriteLine("Received request for latest house data.");
            // Call the method on the injected service to get all received data
            var latestData = _mqttHouseDataService.GetLatestMessages();

            // Check if any data was received
            if (latestData == null || latestData.Count == 0)
            {
                // Return a 404 Not Found response if no data is available
                Console.WriteLine("No house data messages received yet. Returning 404.");
                return NotFound("No house data messages received yet.");
            }

            // Return a 200 OK response with the list of house data messages
            Console.WriteLine($"Returning {latestData.Count} latest house data messages.");
            return Ok(latestData);
        }

        /// <summary>
        /// Retrieves the latest data message for a specific house ID.
        /// </summary>
        /// <param name="propertyId">The unique identifier of the house.</param>
        /// <returns>An ActionResult containing the latest HouseDataMessage for the specified house, or a NotFound result.</returns>
        [HttpGet("{propertyId}")] // Defines a GET endpoint at /api/HouseData/{propertyId}
        public ActionResult<HouseDataMessage> GetHouseDataById(string propertyId)
        {
            Console.WriteLine($"Received request for latest house data for House ID: {propertyId}");

            // Retrieve all received messages from the service
            var allMessages = _mqttHouseDataService.GetLatestMessages();

            // Find the latest message for the specified house ID
            // Note: This assumes the latest message in the list is the most recent for that house.
            // A more robust implementation would sort by timestamp or query a database.
            var latestHouseData = allMessages
                                    .Where(m => m.PropertyId.Equals(propertyId, StringComparison.OrdinalIgnoreCase)) // Filter by House ID (case-insensitive)
                                    .LastOrDefault(); // Get the last (latest) one, or null if none found

            // Check if data for the specific house was found
            if (latestHouseData == null)
            {
                // Return a 404 Not Found response if no data for this house ID is available
                Console.WriteLine($"No house data found for House ID: {propertyId}. Returning 404.");
                return NotFound($"No house data found for House ID: {propertyId}.");
            }

            // Return a 200 OK response with the latest house data message
            Console.WriteLine($"Returning latest house data for House ID: {propertyId}.");
            return Ok(latestHouseData);
        }

        // You could add other endpoints here, e.g., to get all data for a house,
        // filter by time range, etc.
        // [HttpGet("{propertyId}/all")]
        // public ActionResult<List<HouseDataMessage>> GetAllHouseDataById(string propertyId) { ... }
        // [HttpGet("by-time")]
        // public ActionResult<List<HouseDataMessage>> GetHouseDataByTimeRange([FromQuery] DateTime startTime, [FromQuery] DateTime endTime) { ... }
        // Note: Implementing time range filtering would likely require updating IMqttHouseDataService
        // and its implementation to support querying by time.
    }
}

