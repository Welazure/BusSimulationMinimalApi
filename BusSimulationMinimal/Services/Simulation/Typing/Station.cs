namespace BusSimulationMinimal.Services.Simulation.Typing;

public class Station
{
    public string Id { get; set; }
    public string Name { get; set; }

    public double PositionOnRouteKm { get; set; }
    public List<Passenger> WaitingPassengers { get; set; } = new();

    public Guid? currentBusId { get; set; }
    public List<Guid> WaitingBuses { get; set; } = new();
}