using BusSimulationMinimal.Services.Simulation.Typing;

namespace BusSimulationMinimal.Services.Simulation.States;

public class SimulationState
{
    public DateTime CurrentTime { get; set; }
    public bool IsRunning { get; set; }
    public List<Bus> Buses { get; set; } = new();
    public List<Station> Stations { get; set; } = new();
    public double TotalRouteLengthKm { get; set; }

    public SimulationState DeepClone()
    {
        var clone = new SimulationState
        {
            CurrentTime = CurrentTime,
            IsRunning = IsRunning,
            TotalRouteLengthKm = TotalRouteLengthKm,
            Buses = Buses.Select(b => b.DeepClone()).ToList(),
            Stations = Stations.Select(s => s.DeepClone()).ToList()
        };
        return clone;
    }
}