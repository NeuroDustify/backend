using NeuroDustify.Domain.Entities; // Reference to the Domain layer for the entity
using System.Collections.Generic;
using System.Threading.Tasks; // Used for asynchronous operations

namespace NeuroDustify.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for a service responsible for receiving and managing
    /// driveway data messages from an MQTT broker.
    /// </summary>
    public interface IMqttDrivewayDataService
    {
        /// <summary>
        /// Starts the MQTT client, connects to the broker, and subscribes to the driveway
        /// data topic. This should run as a background task.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task StartAsync();

        /// <summary>
        /// Stops the MQTT client and disconnects from the broker.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task StopAsync();

        /// <summary>
        /// Retrieves the latest driveway data messages that have been received
        /// by the service.
        /// </summary>
        /// <returns>A list of DrivewayDataMessage objects.</returns>
        // Note: This returns the raw messages as received. Further processing
        // (e.g., consolidating updates for the same driveway ID) would happen
        // in the Application layer or another service.
        List<DrivewayDataMessage> GetLatestMessages();
    }
}