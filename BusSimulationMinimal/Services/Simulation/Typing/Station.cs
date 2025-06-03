namespace BusSimulationMinimal.Services.Simulation.Typing;

public class Station
{
    public string Id { get; set; }
    public string Name { get; set; }

    public double PositionOnRouteKm { get; set; }
    public List<Passenger> WaitingPassengers { get; set; } = new();

    public Guid? CurrentBusId { get; set; }
    public List<Guid> WaitingBuses { get; set; } = new();

    public int DensityThreshold { get; set; } = 100; // Default value, can be overridden
    
    public bool isOverCrowded => WaitingPassengers.Count > DensityThreshold;

    public Station DeepClone()
    {
        var clone = new Station
        {
            Id = Id,
            Name = Name,
            PositionOnRouteKm = PositionOnRouteKm,
            CurrentBusId = CurrentBusId, // Guid? is a value type, direct copy
            WaitingBuses = new List<Guid>(WaitingBuses), // New list, Guids are value types
            WaitingPassengers = WaitingPassengers.Select(p => p.DeepClone()).ToList() // Deep clone list of passengers
            // Copy other properties
        };
        return clone;
    }
}