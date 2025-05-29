using BusSimulationMinimal.Services.Configuration;
using BusSimulationMinimal.Services.Configuration.Interface;
using BusSimulationMinimal.Services.Simulation;
using BusSimulationMinimal.Services.Simulation.Interface;
using Microsoft.OpenApi.Models;

namespace BusSimulationMinimal;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1",
                new OpenApiInfo { Title = "Bus Simulation API", Version = "v1" });
        });


        builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
        builder.Services.AddSingleton<Orchestrator>();

        builder.Services.AddTransient<IBusService, BusService>();
        builder.Services.AddTransient<IPassengerService, PassengerService>();
        builder.Services.AddTransient<IStationService, StationService>();

        builder.Services.AddHostedService<SimulationBackgroundService>();

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bus Simulation API v1"); });
        }


        app.MapGet("/api/simulation/state", () => { return "hello world"; });
        app.MapGet("/api/simulation/config", () => { return "hello world"; });
        app.MapGet("/api/stations", () => { return "hello world"; });
        app.MapGet("/api/stations/{id}/status", (int id) => { return id; });
        app.MapGet("/api/buses", () => { return "hello world"; });
        app.MapGet("/api/buses/{id}/status", (int id) => { return "hello world" + id; });


        app.MapPost("/api/simulation/start", () => { return "hello world"; });
        app.MapPost("/api/simulation/pause", () => { return "hello world"; });
        app.MapPost("/api/simulation/reset", () => { return "hello world"; });
        app.MapPost("/api/simulation/config", () => { return "hello world"; });
        app.MapPost("/api/buses/dispatch", () => { return "hello world"; });
        app.MapPost("/api/buses/{id}/returnToPool", (int id) => { return "hello world" + id; });


        app.Run();
    }
}