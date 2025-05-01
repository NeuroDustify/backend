// NeuroDustify.WebApi/Controllers/SuburbDataController.cs
// Web API controller for accessing suburb data received via MQTT.
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
    [Route("api/[controller]")] // Defines the base route for this controller (e.g., /api/SuburbData)
    public class SuburbDataController : ControllerBase // Inherits from ControllerBase for API functionality
    {
        // Dependency Injection: The controller depends on the IMqttSuburbDataService interface.
        // The actual implementation (MqttSuburbDataService) will be provided at runtime by the DI container.
        private readonly IMqttSuburbDataService _mqttSuburbDataService;

        /// <summary>
        /// Initializes a new instance of the SuburbDataController.
        /// </summary>
        /// <param name="mqttSuburbDataService">The injected MQTT suburb data service.</param>
        public SuburbDataController(IMqttSuburbDataService mqttSuburbDataService)
        {
            _mqttSuburbDataService = mqttSuburbDataService;
        }

        /// <summary>
        /// Retrieves the latest suburb data messages received via MQTT.
        /// </summary>
        /// <returns>An ActionResult containing a list of SuburbDataMessage objects or a NotFound result.</returns>
        [HttpGet("latest")] // Defines a GET endpoint at /api/SuburbData/latest
        public ActionResult<List<SuburbDataMessage>> GetLatestSuburbData()
        {
            Console.WriteLine("Received request for latest suburb data.");
            // Call the method on the injected service to get all received data
            var latestData = _mqttSuburbDataService.GetLatestMessages();

            // Check if any data was received
            if (latestData == null || latestData.Count == 0)
            {
                // Return a 404 Not Found response if no data is available
                Console.WriteLine("No suburb data messages received yet. Returning 404.");
                return NotFound("No suburb data messages received yet.");
            }

            // Return a 200 OK response with the list of suburb data messages
            Console.WriteLine($"Returning {latestData.Count} latest suburb data messages.");
            return Ok(latestData);
        }

        /// <summary>
        /// Retrieves the latest data message for a specific suburb ID.
        /// </summary>
        /// <param name="suburbId">The unique identifier of the suburb.</param>
        /// <returns>An ActionResult containing the latest SuburbDataMessage for the specified suburb, or a NotFound result.</returns>
        [HttpGet("{suburbId}")] // Defines a GET endpoint at /api/SuburbData/{suburbId}
        public ActionResult<SuburbDataMessage> GetSuburbDataById(string suburbId)
        {
            Console.WriteLine($"Received request for latest suburb data for Suburb ID: {suburbId}");

            // Retrieve all received messages from the service
            var allMessages = _mqttSuburbDataService.GetLatestMessages();

            // Find the latest message for the specified suburb ID
            var latestSuburbData = allMessages
                                        .Where(m => m.SuburbId.Equals(suburbId, StringComparison.OrdinalIgnoreCase)) // Filter by Suburb ID (case-insensitive)
                                        .LastOrDefault(); // Get the last (latest) one, or null if none found

            // Check if data for the specific suburb was found
            if (latestSuburbData == null)
            {
                // Return a 404 Not Found response if no data for this suburb ID is available
                Console.WriteLine($"No suburb data found for Suburb ID: {suburbId}. Returning 404.");
                return NotFound($"No suburb data found for Suburb ID: {suburbId}.");
            }

            // Return a 200 OK response with the latest suburb data message
            Console.WriteLine($"Returning latest suburb data for Suburb ID: {suburbId}.");
            return Ok(latestSuburbData);
        }

        // Additional endpoints can be added here as needed, similar to StreetDataController.
    }
}