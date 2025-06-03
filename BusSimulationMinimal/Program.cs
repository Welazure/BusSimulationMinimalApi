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

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "MyAllowSpecificOrigins",
                policy =>
                {
                    policy.WithOrigins("http://localhost:3000") // Allow your frontend origin
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    // For development, if you might use other ports for frontend:
                    // policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
                    //       .AllowAnyHeader()
                    //       .AllowAnyMethod();

                    // WARNING: For production, be more restrictive with origins, methods, and headers.
                    // DO NOT use AllowAnyOrigin() in production without careful consideration.
                    // policy.AllowAnyOrigin() // Less secure, generally not for production
                    //       .AllowAnyHeader()
                    //       .AllowAnyMethod();
                });
        });

        builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
        builder.Services.AddSingleton<IOrchestrator, NewOrchestrator>();

        builder.Services.AddTransient<IBusService, BusService>();
        builder.Services.AddTransient<IPassengerService, PassengerService>();
        builder.Services.AddTransient<IStationService, StationService>();

        builder.Services.AddHostedService<SimulationBackgroundService>();
        builder.Services.AddHostedService<PollBackgroundService>();

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bus Simulation API v1"); });
        }


        app.UseCors("MyAllowSpecificOrigins");
        app.MapControllers();
        app.Run();
    }
}