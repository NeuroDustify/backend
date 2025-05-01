// NeuroDustify.WebApi/Controllers/DrivewayDataController.cs
// Web API controller for accessing driveway data received via MQTT.
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
    [Route("api/[controller]")] // Defines the base route for this controller (e.g., /api/DrivewayData)
    public class DrivewayDataController : ControllerBase // Inherits from ControllerBase for API functionality
    {
        // Dependency Injection: The controller depends on the IMqttDrivewayDataService interface.
        // The actual implementation (MqttDrivewayDataService) will be provided at runtime by the DI container.
        private readonly IMqttDrivewayDataService _mqttDrivewayDataService;

        /// <summary>
        /// Initializes a new instance of the DrivewayDataController.
        /// </summary>
        /// <param name="mqttDrivewayDataService">The injected MQTT driveway data service.</param>
        public DrivewayDataController(IMqttDrivewayDataService mqttDrivewayDataService)
        {
            _mqttDrivewayDataService = mqttDrivewayDataService;
        }

        /// <summary>
        /// Retrieves the latest driveway data messages received via MQTT.
        /// </summary>
        /// <returns>An ActionResult containing a list of DrivewayDataMessage objects or a NotFound result.</returns>
        [HttpGet("latest")] // Defines a GET endpoint at /api/DrivewayData/latest
        public ActionResult<List<DrivewayDataMessage>> GetLatestDrivewayData()
        {
            Console.WriteLine("Received request for latest driveway data.");
            // Call the method on the injected service to get all received data
            var latestData = _mqttDrivewayDataService.GetLatestMessages();

            // Check if any data was received
            if (latestData == null || latestData.Count == 0)
            {
                // Return a 404 Not Found response if no data is available
                Console.WriteLine("No driveway data messages received yet. Returning 404.");
                return NotFound("No driveway data messages received yet.");
            }

            // Return a 200 OK response with the list of driveway data messages
            Console.WriteLine($"Returning {latestData.Count} latest driveway data messages.");
            return Ok(latestData);
        }

        /// <summary>
        /// Retrieves the latest data message for a specific driveway ID.
        /// </summary>
        /// <param name="drivewayId">The unique identifier of the driveway.</param>
        /// <returns>An ActionResult containing the latest DrivewayDataMessage for the specified driveway, or a NotFound result.</returns>
        [HttpGet("{drivewayId}")] // Defines a GET endpoint at /api/DrivewayData/{drivewayId}
        public ActionResult<DrivewayDataMessage> GetDrivewayDataById(string drivewayId)
        {
            Console.WriteLine($"Received request for latest driveway data for Driveway ID: {drivewayId}");

            // Retrieve all received messages from the service
            var allMessages = _mqttDrivewayDataService.GetLatestMessages();

            // Find the latest message for the specified driveway ID
            var latestDrivewayData = allMessages
                                        .Where(m => m.DrivewayId.Equals(drivewayId, StringComparison.OrdinalIgnoreCase)) // Filter by Driveway ID (case-insensitive)
                                        .LastOrDefault(); // Get the last (latest) one, or null if none found

            // Check if data for the specific driveway was found
            if (latestDrivewayData == null)
            {
                // Return a 404 Not Found response if no data for this driveway ID is available
                Console.WriteLine($"No driveway data found for Driveway ID: {drivewayId}. Returning 404.");
                return NotFound($"No driveway data found for Driveway ID: {drivewayId}.");
            }

            // Return a 200 OK response with the latest driveway data message
            Console.WriteLine($"Returning latest driveway data for Driveway ID: {drivewayId}.");
            return Ok(latestDrivewayData);
        }

        // Additional endpoints can be added here as needed, similar to HouseDataController.
    }
}