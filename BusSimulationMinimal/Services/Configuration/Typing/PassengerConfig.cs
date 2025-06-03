namespace BusSimulationMinimal.Services.Configuration.Typing;

public class PassengerConfig
{
    public int TimeToLiveSeconds { get; set; }
    public int BasePassengerSpawnRate { get; set; }
    public List<TimeMultiplierPoint> GenerationMultiplierGraph { get; set; }
    public Dictionary<string, List<DestinationWeightPoint>> DestinationWeights { get; set; }
}