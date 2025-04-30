// NeuroDustify.Application/Interfaces/IMqttHouseDataService.cs
// Defines the interface for a service that handles house data received via MQTT.
// This belongs in the Application layer.

using NeuroDustify.Domain.Entities; // Reference to the Domain layer for the entity
using System.Collections.Generic;
using System.Threading.Tasks; // Used for asynchronous operations

namespace NeuroDustify.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for a service responsible for receiving and managing
    /// house data messages from an MQTT broker.
    /// </summary>
    public interface IMqttHouseDataService
    {
        /// <summary>
        /// Starts the MQTT client and connects to the broker, initiating the subscription
        /// to the house data topic. This should run as a background task.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task StartAsync();

        /// <summary>
        /// Stops the MQTT client and disconnects from the broker.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task StopAsync();

        /// <summary>
        /// Retrieves the latest collection of house data messages that have been received
        /// by the service.
        /// </summary>
        /// <returns>A list of HouseDataMessage objects.</returns>
        // Note: This returns the raw messages as received. Further processing
        // (e.g., consolidating updates for the same house ID) would happen
        // in the Application layer or another service.
        List<HouseDataMessage> GetLatestMessages();

        // You could add other methods here later based on application needs,
        // e.g., GetLatestMessageForHouse(string propertyId)
    }
}
