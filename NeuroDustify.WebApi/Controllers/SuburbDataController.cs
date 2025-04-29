// // NeuroDustify.WebApi/Controllers/SuburbDataController.cs
// // Web API controller for accessing suburb data received via MQTT.
// // This controller belongs in the WebApi (Presentation) layer.

// using Microsoft.AspNetCore.Mvc; // Base classes for MVC/API controllers
// using NeuroDustify.Application.Interfaces; // Reference to the Application layer interface (assuming a similar interface exists for suburb data)
// using NeuroDustify.Domain.Entities; // Reference to the Domain layer entities (assuming a SuburbDataMessage or similar entity)
// using System.Collections.Generic; // For List
// using System.Linq; // For LINQ methods like Where, FirstOrDefault
// using System; // For Console

// // Assuming these namespaces and entities exist in your project structure,
// // similar to how they are used for BinDataController.

// // Make sure you have a corresponding interface like IMqttSuburbDataService
// // in your Application.Interfaces project
// // and a SuburbDataMessage or similar class
// // in your Domain.Entities project.

// namespace NeuroDustify.WebApi.Controllers
// {
//     [ApiController] // Indicates that this class is an API controller
//     [Route("api/[controller]")] // Defines the base route for this controller (e.g., /api/SuburbData)
//     public class SuburbDataController : ControllerBase // Inherits from ControllerBase for API functionality
//     {
//         // Dependency Injection: The controller depends on an interface for suburb data.
//         // The actual implementation will be provided at runtime by the DI container.
//         private readonly IMqttSuburbDataService _mqttSuburbDataService;

//         /// <summary>
//         /// Initializes a new instance of the SuburbDataController.
//         /// </summary>
//         /// <param name="mqttSuburbDataService">The injected MQTT suburb data service.</param>
//         public SuburbDataController(IMqttSuburbDataService mqttSuburbDataService)
//         {
//             _mqttSuburbDataService = mqttSuburbDataService;
//         }

//         /// <summary>
//         /// Retrieves the latest suburb data messages received via MQTT.
//         /// </summary>
//         /// <returns>An ActionResult containing a list of SuburbDataMessage objects or a NotFound result.</returns>
//         [HttpGet("latest")] // Defines a GET endpoint at /api/SuburbData/latest
//         public ActionResult<List<SuburbDataMessage>> GetLatestSuburbData()
//         {
//             Console.WriteLine("Received request for latest suburb data.");
//             // Call the method on the injected service to get all received data
//             var latestData = _mqttSuburbDataService.GetLatestMessages();

//             // Check if any data was received
//             if (latestData == null || latestData.Count == 0)
//             {
//                 // Return a 404 Not Found response if no data is available
//                 Console.WriteLine("No suburb data messages received yet. Returning 404.");
//                 return NotFound("No suburb data messages received yet.");
//             }

//             // Return a 200 OK response with the list of suburb data messages
//             Console.WriteLine($"Returning {latestData.Count} latest suburb data messages.");
//             return Ok(latestData);
//         }

//         /// <summary>
//         /// Retrieves the latest data message for a specific suburb ID (or name, depending on your entity structure).
//         /// </summary>
//         /// <param name="suburbId">The unique identifier (or name) of the suburb.</param>
//         /// <returns>An ActionResult containing the latest SuburbDataMessage for the specified suburb, or a NotFound result.</returns>
//         [HttpGet("{suburbId}")] // Defines a GET endpoint at /api/SuburbData/{suburbId}
//         public ActionResult<SuburbDataMessage> GetSuburbDataById(string suburbId)
//         {
//             Console.WriteLine($"Received request for latest suburb data for Suburb ID: {suburbId}");

//             // Retrieve all received messages from the service
//             var allMessages = _mqttSuburbDataService.GetLatestMessages();

//             // Find the latest message for the specified suburb ID
//             // Note: This assumes the latest message in the list is the most recent for that suburb.
//             // A more robust implementation would sort by timestamp or query a database.
//             var latestSuburbData = allMessages
//                                 .Where(m => m.SuburbId.Equals(suburbId, StringComparison.OrdinalIgnoreCase)) // Filter by Suburb ID (case-insensitive) - Adjust property name based on your entity
//                                 .OrderByDescending(m => m.Timestamp) // Order by timestamp descending - Adjust property name if needed
//                                 .FirstOrDefault(); // Get the first (latest) one, or null if none found

//             // Check if data for the specific suburb was found
//             if (latestSuburbData == null)
//             {
//                 // Return a 404 Not Found response if no data for this suburb is available
//                 Console.WriteLine($"No suburb data found for Suburb ID: {suburbId}. Returning 404.");
//                 return NotFound($"No suburb data found for Suburb ID: {suburbId}.");
//             }

//             // Return a 200 OK response with the latest suburb data message
//             Console.WriteLine($"Returning latest suburb data for Suburb ID: {suburbId}.");
//             return Ok(latestSuburbData);
//         }

//         // You could add other endpoints here, e.g., to get all data for a suburb,
//         // filter by time range, etc.

//         // Remember to implement IMqttSuburbDataService and the SuburbDataMessage entity
//         // in your Application and Domain layers respectively.
//     }

//     // Placeholder for your SuburbDataMessage entity - define this in your Domain.Entities project
//     // public class SuburbDataMessage
//     // {
//     //     public string SuburbId { get; set; } // Or SuburbName, adjust as needed
//     //     public DateTime Timestamp { get; set; }
//     //     // Add other properties relevant to your suburb data (e.g., number of houses, streets, etc.)
//     // }

//      // Placeholder for your IMqttSuburbDataService interface - define this in your Application.Interfaces project
//      // public interface IMqttSuburbDataService
//      // {
//      //     List<SuburbDataMessage> GetLatestMessages();
//      //     // Add other methods as needed for retrieving suburb data
//      // }
// }