namespace BusSimulationMinimal.Services.Configuration.Typing;

public class RouteConfig
{
    public double TotalRouteLengthKm { get; set; }
    public List<StationConfig> Stations { get; set; }
}