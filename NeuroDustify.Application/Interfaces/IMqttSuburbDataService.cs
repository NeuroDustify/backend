// NeuroDustify.Application/Interfaces/IMqttSuburbDataService.cs
// Defines the interface for a service that handles house data received via MQTT.
// This belongs in the Application layer.

using NeuroDustify.Domain.Entities; // Reference to the Domain layer for the entity
using System.Collections.Generic;
using System.Threading.Tasks; // Used for asynchronous operations

namespace NeuroDustify.Application.Interfaces
{
    /// <Summary>
    /// Defines the contract for a service respinsible for recieving and managing
    /// street data messages from an MQTT broker.
    /// </summary>
    public interface IMqttSuburbDataService 
    {
        /// <summary>
        /// Starts the MQTT client and connects to the broker, initiating the subscription
        /// to the street data topic. This should run as a background task.
        /// </summary>
        /// <retrns>A Task representing the asynchronous oeration.</returns>
        Task StartAsync();

        /// <summary> 
        /// Stops the MQTT Client and disconnects from the broker.
        /// </summary>
        /// <returns>A Task representing the asynchronous operations.</returns>
        Task StopAsync();

        /// <summary>
        /// Retrieves the lates collection of the Suburb data messages that have been recieved
        /// by the service.
        /// </summary>
        /// <returns>A list of SuburbDataMessage objects.</returns>
        List<SuburbDataMessage> GetLatestMessages();
    }   
}