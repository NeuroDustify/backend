// NeuroDustify.WebApi/Controllers/StreetDataController.cs
// Web API controller for accessing street data received via MQTT.
// This controller belongs in the WebApi (Presentation) layer.

using Microsoft.AspNetCore.Mvc; // Base classes for MVC/API controllers
using NeuroDustify.Application.Interfaces; // Reference to the Application layer interface
using NeuroDustify.Domain.Entities; // Reference to the Domain layer entities
using System.Collections.Generic; // For List
using System.Linq; // For LINQ methods like Where, LastOrDefault
using System; // For Console

namespace NeuroDustify.WebApi.Controllers
{
    [ApiController] // Indicates that this class is an API controller
    [Route("api/[controller]")] // Defines the base route for this controller (e.g., /api/StreetData)
    public class StreetDataController : ControllerBase // Inherits from ControllerBase for API functionality
    {
        // Dependency Injection: The controller depends on the IMqttStreetDataService interface.
        // The actual implementation (MqttStreetDataService) will be provided at runtime by the DI container.
        private readonly IMqttStreetDataService _mqttStreetDataService;

        /// <summary>
        /// Initializes a new instance of the StreetDataController.
        /// </summary>
        /// <param name="mqttStreetDataService">The injected MQTT street data service.</param>
        public StreetDataController(IMqttStreetDataService mqttStreetDataService)
        {
            _mqttStreetDataService = mqttStreetDataService;
        }

        /// <summary>
        /// Retrieves the latest street data messages received via MQTT.
        /// </summary>
        /// <returns>An ActionResult containing a list of StreetDataMessage objects or a NotFound result.</returns>
        [HttpGet("latest")] // Defines a GET endpoint at /api/StreetData/latest
        public ActionResult<List<StreetDataMessage>> GetLatestStreetData()
        {
            Console.WriteLine("Received request for latest street data.");
            // Call the method on the injected service to get all received data
            var latestData = _mqttStreetDataService.GetLatestMessages();

            // Check if any data was received
            if (latestData == null || latestData.Count == 0)
            {
                // Return a 404 Not Found response if no data is available
                Console.WriteLine("No street data messages received yet. Returning 404.");
                return NotFound("No street data messages received yet.");
            }

            // Return a 200 OK response with the list of street data messages
            Console.WriteLine($"Returning {latestData.Count} latest street data messages.");
            return Ok(latestData);
        }

        /// <summary>
        /// Retrieves the latest data message for a specific street ID.
        /// </summary>
        /// <param name="streetId">The unique identifier of the street.</param>
        /// <returns>An ActionResult containing the latest StreetDataMessage for the specified street, or a NotFound result.</returns>
        [HttpGet("{streetId}")] // Defines a GET endpoint at /api/StreetData/{streetId}
        public ActionResult<StreetDataMessage> GetStreetDataById(string streetId)
        {
            Console.WriteLine($"Received request for latest street data for Street ID: {streetId}");

            // Retrieve all received messages from the service
            var allMessages = _mqttStreetDataService.GetLatestMessages();

            // Find the latest message for the specified street ID
            var latestStreetData = allMessages
                                        .Where(m => m.StreetId.Equals(streetId, StringComparison.OrdinalIgnoreCase)) // Filter by Street ID (case-insensitive)
                                        .LastOrDefault(); // Get the last (latest) one, or null if none found

            // Check if data for the specific street was found
            if (latestStreetData == null)
            {
                // Return a 404 Not Found response if no data for this street ID is available
                Console.WriteLine($"No street data found for Street ID: {streetId}. Returning 404.");
                return NotFound($"No street data found for Street ID: {streetId}.");
            }

            // Return a 200 OK response with the latest street data message
            Console.WriteLine($"Returning latest street data for Street ID: {streetId}.");
            return Ok(latestStreetData);
        }

        // Additional endpoints can be added here as needed, similar to DrivewayDataController.
    }
}