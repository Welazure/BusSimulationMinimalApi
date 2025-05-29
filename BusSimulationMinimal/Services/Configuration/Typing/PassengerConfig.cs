namespace BusSimulationMinimal.Services.Configuration.Typing;

public class PassengerConfig
{
    public int basePassengerSpawnRate { get; set; }
    public List<TimeMultiplierPoint> generationMultiplierGraph { get; set; }
    public Dictionary<string, List<DestinationWeightPoint>> DestinationWeights { get; set; }
}