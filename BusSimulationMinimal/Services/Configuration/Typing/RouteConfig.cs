using BusSimulationMinimal.Services.Simulation.Typing;

namespace BusSimulationMinimal.Services.Configuration.Typing;

public class RouteConfig
{
    public double TotalRouteLengthKm { get; set; }
    public Station[] Stations { get; set; }
}