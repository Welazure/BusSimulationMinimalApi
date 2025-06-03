namespace BusSimulationMinimal.Models;

public class DispatchBusRequestDto
{
    public int? Capacity { get; set; }
    public string? StartAtStationId { get; set; }
    public string? Direction { get; set; } // Will be "FORWARD" or "BACKWARD"
}