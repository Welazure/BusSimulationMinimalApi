using BusSimulationMinimal.Services.Simulation.Typing;

namespace BusSimulationMinimal.Services.Configuration.Typing;

public class RouteConfig
{
    public double totalRouteLengthKm { get; set; }
    public Station[] stations { get; set; }
}