// NeuroDustify.WebApi/Controllers/BinDataController.cs
// Web API controller for accessing bin data received via MQTT.
// This controller belongs in the WebApi (Presentation) layer.

using Microsoft.AspNetCore.Mvc; // Base classes for MVC/API controllers
using NeuroDustify.Application.Interfaces; // Reference to the Application layer interface
using NeuroDustify.Domain.Entities; // Reference to the Domain layer entities
using System.Collections.Generic; // For List

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
            // Call the method on the injected service to get the data
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

        // You could add other endpoints here to expose different functionalities,
        // e.g., getting data for a specific bin, filtering by time, etc.
        // [HttpGet("{binId}")]
        // public ActionResult<BinDataMessage> GetBinDataById(string binId) { ... }
    }
}
