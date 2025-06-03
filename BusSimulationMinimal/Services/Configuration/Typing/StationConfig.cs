namespace BusSimulationMinimal.Services.Configuration.Typing;

public class StationConfig(string Id, string Name, double PositionOnRouteKm, int DensityThreshold)
{
    public string Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public double PositionOnRouteKm { get; set; } = PositionOnRouteKm;
    public int DensityThreshold { get; set; } = DensityThreshold;
}