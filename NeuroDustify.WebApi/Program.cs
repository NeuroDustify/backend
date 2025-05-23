// NeuroDustify.WebApi/Program.cs
// Entry point and configuration for the NeuroDustify Web API.
// This file sets up the application host, configures services, and defines the request pipeline.

using Microsoft.AspNetCore.Builder; // For WebApplicationBuilder and WebApplication
using Microsoft.Extensions.DependencyInjection; // For AddControllers, AddSingleton, AddHostedService
using Microsoft.Extensions.Hosting; // For IHostedService, Host
using Microsoft.Extensions.Configuration; // For accessing configuration (e.g., appsettings.json)
using NeuroDustify.Application.Interfaces; // Reference to the Application layer interface
using NeuroDustify.Infrastructure.Mqtt; // Reference to the Infrastructure layer implementation
using System; // For Console
using System.Threading; // For CancellationToken
using System.Threading.Tasks; // For Task
using Swashbuckle.AspNetCore; // For SwaggerGen

// Create a new WebApplication builder.
// This builder is used to configure the application's services and request pipeline.
var builder = WebApplication.CreateBuilder(args);

// --- Configure Services (Dependency Injection Container) ---
// Services are registered here and can be injected into constructors later.
// This is where you wire up the dependencies between your layers.

// Add controllers to the service collection. This enables MVC controllers (which includes API controllers)
// in the application and makes them available for dependency injection.
builder.Services.AddControllers();

// Add services for exploring endpoints and generating Swagger/OpenAPI documentation.
// This is typically used for API discovery and testing tools like Swagger UI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure and register the MQTT Bin Data Service.
// We register it as a Singleton because we want a single instance of the MQTT client
// to run throughout the application's lifetime, managing the connection and received data.
// We retrieve configuration values from appsettings.json using builder.Configuration.
var mqttBrokerAddress = builder.Configuration["Mqtt:BrokerAddress"] ?? "test.mosquitto.org"; // Get broker address from config, default if not found
var mqttPort = builder.Configuration.GetValue<int>("Mqtt:Port", 1883); // Get port from config, default if not found
var mqttTopic = builder.Configuration["Mqtt:Topic"] ?? "suburb/model/igention/bins"; // Get topic from config, default if not found

Console.WriteLine($"Configuring MqttBinDataService with Broker: {mqttBrokerAddress}:{mqttPort}, Topic: {mqttTopic}");

// Register the MqttBinDataService implementation for the IMqttBinDataService interface.
// When a class requests an IMqttBinDataService, the DI container will provide a single instance
// of MqttBinDataService, created with the specified configuration values.
builder.Services.AddSingleton<IMqttBinDataService>(sp =>
    new MqttBinDataService(mqttBrokerAddress, mqttPort, mqttTopic));

// Register the MqttBinDataHostedService.
// IHostedService implementations are automatically started and stopped by the .NET host
// when the application starts and stops. This is the standard and recommended way to run
// long-running background tasks like an MQTT client listener in ASP.NET Core/ .NET applications.
// The hosted service will resolve the IMqttBinDataService from the DI container and manage its lifecycle.
builder.Services.AddHostedService<MqttBinDataHostedService>();

// Configure and register the MQTT House Data Service.
// We register it as a Singleton because we want a single instance of the MQTT client
// to run throughout the application's lifetime, managing the connection and received data.
// We retrieve configuration values from appsettings.json using builder.Configuration.
var houseMqttBrokerAddress = builder.Configuration["MqttHouse:BrokerAddress"] ?? "test.mosquitto.org"; // Get broker address from config, default if not found
var houseMqttPort = builder.Configuration.GetValue<int>("MqttHouse:Port", 1883); // Get port from config, default if not found
var houseMqttTopic = builder.Configuration["MqttHouse:Topic"] ?? "suburb/model/igention/houses"; // Get topic from config, default if not found

Console.WriteLine($"Configuring MqttHouseDataService with Broker: {houseMqttBrokerAddress}:{houseMqttPort}, Topic: {houseMqttTopic}");

// Register the MqttHouseDataService implementation for the IMqttHouseDataService interface.
// When a class requests an IMqttHouseDataService, the DI container will provide a single instance
// of MqttHouseDataService, created with the specified configuration values.
builder.Services.AddSingleton<IMqttHouseDataService>(sp =>
    new MqttHouseDataService(houseMqttBrokerAddress, houseMqttPort, houseMqttTopic));

// Register the MqttHouseDataHostedService.
// IHostedService implementations are automatically started and stopped by the .NET host
// when the application starts and stops. This is the standard and recommended way to run
// long-running background tasks like an MQTT client listener in ASP.NET Core/ .NET applications.
// The hosted service will resolve the IMqttHouseDataService from the DI container and manage its lifecycle.
builder.Services.AddHostedService<MqttHouseDataHostedService>();

// Configure and register the MQTT Driveway Service.
// We register it as a Singleton because we want a single instance of the MQTT client
// to run throughout the application's lifetime, managing the connection and received data.
// We retrieve configuration values from appsettings.json using builder.Configuration.
var drivewayMqttBrokerAddress = builder.Configuration["MqttDriveway:BrokerAddress"] ?? "test.mosquitto.org"; // Get broker address from config, default if not found
var drivewayMqttPort = builder.Configuration.GetValue<int>("MqttDriveway:Port", 1883); // Get port from config, default if not found
var drivewayMqttTopic = builder.Configuration["MqttDriveway:Topic"] ?? "suburb/model/igention/driveways"; // Get topic from config, default if not found

Console.WriteLine($"Configuring MqttDrivewayService with Broker: {drivewayMqttBrokerAddress}:{drivewayMqttPort}, Topic: {drivewayMqttTopic}");

// Register the MqttDrivewayDataService implementation for the IMqttDrivewayDataService interface.
// When a class requests an IMqttDrivewayDataService, the DI container will provide a single instance
// of MqttBinDataService, created with the specified configuration values.
builder.Services.AddSingleton<IMqttDrivewayDataService>(sp =>
    new MqttDrivewayDataService(drivewayMqttBrokerAddress, drivewayMqttPort, drivewayMqttTopic));

// Register the MqttDrivewayHostedService.
// IHostedService implementations are automatically started and stopped by the .NET host
// when the application starts and stops. This is the standard and recommended way to run
// long-running background tasks like an MQTT client listener in ASP.NET Core/ .NET applications.
// The hosted service will resolve the IMqttDrivewayDataService from the DI container and manage its lifecycle.
builder.Services.AddHostedService<MqttDrivewayDataHostedService>();

// Configure and register the MQTT Driveway Service.
// We register it as a Singleton because we want a single instance of the MQTT client
// to run throughout the application's lifetime, managing the connection and received data.
// We retrieve configuration values from appsettings.json using builder.Configuration.
var streetMqttBrokerAddress = builder.Configuration["MqttStreet:BrokerAddress"] ?? "test.mosquitto.org"; // Get broker address from config, default if not found
var streetMqttPort = builder.Configuration.GetValue<int>("MqttStreet:Port", 1883); // Get port from config, default if not found
var streetMqttTopic = builder.Configuration["MqttStreet:Topic"] ?? "suburb/model/igention/streets"; // Get topic from config, default if not found

Console.WriteLine($"Configuring MqttStreetDataService with Broker: {streetMqttBrokerAddress}:{streetMqttPort}, Topic: {streetMqttTopic}");

// Register the MqttStreetDataService implementation for the IMqttStreetDataService interface.
// When a class requests an IMqttStreetDataService, the DI container will provide a single instance
// of MqttBinDataService, created with the specified configuration values.
builder.Services.AddSingleton<IMqttStreetDataService>(sp =>
    new MqttStreetDataService(streetMqttBrokerAddress, streetMqttPort, streetMqttTopic));

// Register the MqttStreetHostedService.
// IHostedService implementations are automatically started and stopped by the .NET host
// when the application starts and stops. This is the standard and recommended way to run
// long-running background tasks like an MQTT client listener in ASP.NET Core/ .NET applications.
// The hosted service will resolve the IMqttBinDataService from the DI container and manage its lifecycle.
builder.Services.AddHostedService<MqttStreetDataHostedService>();

// Configure and register the MQTT Suburb Service.
// We register it as a Singleton because we want a single instance of the MQTT client
// to run throughout the application's lifetime, managing the connection and received data.
// We retrieve configuration values from appsettings.json using builder.Configuration.
var suburbMqttBrokerAddress = builder.Configuration["MqttSuburb:BrokerAddress"] ?? "test.mosquitto.org";
var suburbMqttPort = builder.Configuration.GetValue<int>("MqttSuburb:Port", 1883);
var suburbMqttTopic = builder.Configuration["MqttSuburb:Topic"] ?? "suburb/model/igention/suburb";

Console.WriteLine($"Configuring MqttSuburbDataService with Broker: {suburbMqttBrokerAddress}:{suburbMqttPort}, Topic: {suburbMqttTopic}");

// Register the MqttSuburbDataService implementation for the IMqttSuburbDataService interface.
// When a class requests an IMqttSuburbDataService, the DI container will provide a single instance
// of MqttSuburbDataService, created with the specified configuration values.
builder.Services.AddSingleton<IMqttSuburbDataService>(sp =>
    new MqttSuburbDataService(suburbMqttBrokerAddress, suburbMqttPort, suburbMqttTopic));

// Register the MqttSuburbHostedService.
// IHostedService implementations are automatically started and stopped by the .NET host
// when the application starts and stops. This is the standard and recommended way to run
// long-running background tasks like an MQTT client listener in ASP.NET Core/ .NET applications.
// The hosted service will resolve the IMqttSuburbDataService from the DI container and manage its lifecycle.
builder.Services.AddHostedService<MqttSuburbDataHostedService>();

// --- Build the Application ---
// Build the WebApplication instance from the configured builder.
var app = builder.Build();

// --- Configure the HTTP request pipeline ---
// This defines how the application handles incoming HTTP requests.
// Middleware components are added to the pipeline here.

// Configure Swagger UI in the development environment.
// This provides a web-based UI to explore and test the API endpoints.
// Use app.UseDeveloperExceptionPage() here in development for detailed error info.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.UseDeveloperExceptionPage(); // Optional: Add developer exception page for detailed errors in development
}

// Optional: Middleware to redirect HTTP requests to HTTPS.
// app.UseHttpsRedirection();

// Optional: Middleware for authorization.
// app.UseAuthorization();

// Maps incoming requests to the controller endpoints defined using attributes like [ApiController] and [Route].
app.MapControllers();

// --- Run the Application ---
// Starts the application host and listens for incoming requests.
// This is a blocking call that runs until the application is stopped (e.g., via Ctrl+C).
app.Run();


// --- Hosted Service Definition ---
// This nested class is defined here within the Program.cs file.
// It acts as a wrapper to integrate the MqttBinDataService into the IHostedService lifecycle.
// This is a common pattern in modern .NET applications.

/// <summary>
/// A hosted service that manages the lifecycle (Start/Stop) of the MqttBinDataService.
/// Ensures the MQTT client connects and disconnects with the application host's lifetime.
/// This allows the MQTT service to run continuously in the background.
/// </summary>
public class MqttBinDataHostedService : IHostedService
{
    // The IMqttBinDataService instance is injected here by the DI container.
    private readonly IMqttBinDataService _mqttBinDataService;

    /// <summary>
    /// Initializes a new instance of the MqttBinDataHostedService.
    /// </summary>
    /// <param name="mqttBinDataService">The MQTT bin data service instance to manage (injected by DI).</param>
    public MqttBinDataHostedService(IMqttBinDataService mqttBinDataService)
    {
        _mqttBinDataService = mqttBinDataService;
    }

    /// <summary>
    /// Called by the .NET host when the application is starting.
    /// This method is responsible for starting the underlying MQTT bin data service.
    /// </summary>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A Task representing the asynchronous operation of starting the service.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("MqttBinDataHostedService is starting the MQTT Bin Data Service.");
        // Call the StartAsync method of the underlying MQTT service implementation.
        return _mqttBinDataService.StartAsync();
    }

    /// <summary>
    /// Called by the .NET host when the application is stopping.
    /// This method is responsible for stopping the underlying MQTT bin data service.
    /// </summary>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A Task representing the asynchronous operation of stopping the service.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("MqttBinDataHostedService is stopping the MQTT Bin Data Service.");
        // Call the StopAsync method of the underlying MQTT service implementation.
        return _mqttBinDataService.StopAsync();
    }
}

public class MqttDrivewayDataHostedService : IHostedService
{
    // The IMqttHouseDataService instance is injected here by the DI container.
    private readonly IMqttDrivewayDataService _mqttDrivewayDataService;

    /// <summary>
    /// Initializes a new instance of the MqttHouseDataHostedService.
    /// </summary>
    /// <param name="mqttDrivewayDataService">The MQTT house data service instance to manage (injected by DI).</param>
    public MqttDrivewayDataHostedService(IMqttDrivewayDataService mqttDrivewayDataService)
    {
        _mqttDrivewayDataService = mqttDrivewayDataService;
    }

    /// <summary>
    /// Called by the .NET host when the application is starting.
    /// This method is responsible for starting the underlying MQTT house data service.
    /// </summary>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A Task representing the asynchronous operation of starting the service.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("MqttDrivewayDataHostedService is starting the MQTT House Data Service.");
        // Call the StartAsync method of the underlying MQTT service implementation.
        return _mqttDrivewayDataService.StartAsync();
    }

    /// <summary>
    /// Called by the .NET host when the application is stopping.

    /// This method is responsible for stopping the underlying MQTT house data service.
    /// </summary>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A Task representing the asynchronous operation of stopping the service.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("MqttDrivewayDataHostedService is stopping the MQTT House Data Service.");
        // Call the StopAsync method of the underlying MQTT service implementation.
        return _mqttDrivewayDataService.StopAsync();
    }
}

public class MqttStreetDataHostedService : IHostedService
{
    // The IMqttHouseDataService instance is injected here by the DI container.
    private readonly IMqttStreetDataService _mqttStreetDataService;

    /// <summary>
    /// Initializes a new instance of the MqttHouseDataHostedService.
    /// </summary>
    /// <param name="mqttStreetDataService">The MQTT house data service instance to manage (injected by DI).</param>
    public MqttStreetDataHostedService(IMqttStreetDataService mqttStreetDataService)
    {
        _mqttStreetDataService = mqttStreetDataService;
    }

    /// <summary>
    /// Called by the .NET host when the application is starting.
    /// This method is responsible for starting the underlying MQTT house data service.
    /// </summary>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A Task representing the asynchronous operation of starting the service.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("MqttStreetDataHostedService is starting the MQTT House Data Service.");
        // Call the StartAsync method of the underlying MQTT service implementation.
        return _mqttStreetDataService.StartAsync();
    }

    /// <summary>
    /// Called by the .NET host when the application is stopping.

    /// This method is responsible for stopping the underlying MQTT house data service.
    /// </summary>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A Task representing the asynchronous operation of stopping the service.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("MqttStreetDataHostedService is stopping the MQTT House Data Service.");
        // Call the StopAsync method of the underlying MQTT service implementation.
        return _mqttStreetDataService.StopAsync();
    }
}

public class MqttSuburbDataHostedService : IHostedService
{
    // The IMqttSuburbDataService instance is injected here by the DI container.
    private readonly IMqttSuburbDataService _mqttSuburbDataService;

    /// <summary>
    /// Initializes a new instance of the MqttSuburbDataHostedService.
    /// </summary>
    /// <param name="mqttSuburbDataService">The MQTT suburb data service instance to manage (injected by DI).</param>
    public MqttSuburbDataHostedService(IMqttSuburbDataService mqttSuburbDataService)
    {
        _mqttSuburbDataService = mqttSuburbDataService;
    }

    /// <summary>
    /// Called by the .NET host when the application is starting.
    /// This method is responsible for starting the underlying MQTT suburb data service.
    /// </summary>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A Task representing the asynchronous operation of starting the service.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("MqttSuburbDataHostedService is starting the MQTT Suburb Data Service.");
        // Call the StartAsync method of the underlying MQTT service implementation.
        return _mqttSuburbDataService.StartAsync();
    }

    /// <summary>
    /// Called by the .NET host when the application is stopping.
    /// This method is responsible for stopping the underlying MQTT suburb data service.
    /// </summary>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A Task representing the asynchronous operation of stopping the service.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("MqttSuburbDataHostedService is stopping the MQTT Suburb Data Service.");
        // Call the StopAsync method of the underlying MQTT service implementation.
        return _mqttSuburbDataService.StopAsync();
    }
}