// NeuroDustify.Application/Interfaces/IMqttBinDataService.cs
// Defines the interface for a service that handles MQTT bin data.
// This belongs in the Application layer to keep it independent of
// the specific MQTT implementation details.

using NeuroDustify.Domain.Entities; // Reference to the Domain layer for the entity
using System.Collections.Generic;
using System.Threading.Tasks; // Used for asynchronous operations

namespace NeuroDustify.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for a service responsible for receiving and managing
    /// smart bin data messages from an MQTT broker.
    /// </summary>
    public interface IMqttBinDataService
    {
        /// <summary>
        /// Starts the MQTT client and connects to the broker, initiating the subscription
        /// to the bin data topic. This should run as a background task.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task StartAsync();

        /// <summary>
        /// Stops the MQTT client and disconnects from the broker.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task StopAsync();

        /// <summary>
        /// Retrieves the latest collection of bin data messages that have been received
        /// by the service.
        /// </summary>
        /// <returns>A list of BinDataMessage objects.</returns>
        // Note: In a real application, this might be more sophisticated, e.g.,
        // taking parameters for filtering (by Bin ID, time range) or returning
        // a queryable collection.
        List<BinDataMessage> GetLatestMessages();

        // You could add other methods here later based on application needs,
        // e.g., Task SendCommandToBinAsync(string binId, string command);
    }
}
