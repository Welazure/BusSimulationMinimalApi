namespace BusSimulationMinimal.Services.Configuration.Typing;

public class SimulationConfig
{
    public double BusSpeedKmh { get; set; }
    public float BaseStationStopTimeSeconds { get; set; }
    public int PassengerInteractionPerSecond { get; set; }
    public double TickIntervalMilliseconds { get; set; }
    public int DefaultBusCapacity { get; set; }
    public float PassengerSpawnJitterFactor { get; set; }
    public int MaximumBusWaitTimeSeconds { get; set; }
}