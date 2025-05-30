namespace BusSimulationMinimal.Services.Simulation.Typing;

public class Passenger
{
    public Guid Id { get; set; }
    public string OriginStationId { get; set; }
    public string DestinationStationId { get; set; }
    public PassengerStatus Status { get; set; }

    public Passenger DeepClone()
    {
        return new Passenger
        {
            Id = Id,
            OriginStationId = OriginStationId,
            DestinationStationId = DestinationStationId,
            Status = Status
            // Copy other properties
        };
    }
}