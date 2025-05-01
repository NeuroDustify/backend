// NeuroDustify.Application/Interfaces/IMqttStreetDataService.cs
// Defines the interface for a service that handles house data received via MQTT.
// This belongs in the Application layer.

using NeuroDustify.Domain.Entities; // Reference to the Domain layer for the entity
using System.Collections.Generic;
using System.Threading.Tasks; // Used for asynchronous operations

namespace NeuroDustify.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for a service responsible for receiving and managing
    /// street data messages from an MQTT broker.
    /// </summary>
    public interface IMqttStreetDataService
    {
        /// <summary>
        /// Starts the MQTT client and connects to the broker, initiating the subscription
        /// to the street data topic. This should run as a background task.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task StartAsync();

        /// <summary>
        /// Stops the MQTT client and disconnects from the broker.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task StopAsync();

        /// <summary>
        /// Retrieves the latest collection of street data messages that have been received
        /// by the service.
        /// </summary>
        /// <returns>A list of StreetDataMessage objects.</returns>
        List<StreetDataMessage> GetLatestMessages();
    }
}