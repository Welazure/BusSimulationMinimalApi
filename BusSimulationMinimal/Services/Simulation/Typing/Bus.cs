namespace BusSimulationMinimal.Services.Simulation.Typing;

public class Bus
{
    public Guid Id { get; set; }
    public double PositionOnRouteKm { get; set; }
    public int Capacity { get; set; }
    public List<Passenger> Passengers { get; set; } = new();
    public BusStatus Status { get; set; }
    public string? CurrentStationId { get; set; }
    public string? NextStationId { get; set; }

    public double TimeSpentAtStationSec { get; set; }
    public bool ReturningToPool { get; set; }
}