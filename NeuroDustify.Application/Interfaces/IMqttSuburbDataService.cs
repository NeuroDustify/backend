// using NeuroDustify.Domain.Entities; // Reference to the Domain layer for the entity
// using System.Threading.Tasks; // Used for asynchronous operations

// namespace NeuroDustify.Application.Interfaces
// {
//     /// <summary>
//     /// Defines the contract for a service responsible for receiving and managing
//     /// suburb data messages from an MQTT broker.
//     /// </summary>
//     public interface IMqttSuburbDataService
//     {
//         /// <summary>
//         /// Starts the MQTT client, connects to the broker, and subscribes to the suburb
//         /// data topic.  This should run as a background task.
//         /// </summary>
//         /// <returns>A Task representing the asynchronous operation.</returns>
//         Task StartAsync();

//         /// <summary>
//         /// Stops the MQTT client and disconnects from the broker.
//         /// </summary>
//         /// <returns>A Task representing the asynchronous operation.</returns>
//         Task StopAsync();

//         /// <summary>
//         /// Retrieves the latest suburb data message that has been received by the service.
//         /// Since suburb data is published as a single message, we return a single object.
//         /// </summary>
//         /// <returns>The SuburbDataMessage object, or null if no message has been received.</returns>
//         SuburbDataMessage? GetLatestSuburbData(); // Changed return type to SuburbDataMessage?
//     }
// }