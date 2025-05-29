using BusSimulationMinimal.Services.Configuration.Typing;
using BusSimulationMinimal.Services.Simulation.Typing;

namespace BusSimulationMinimal.Services.Simulation.States;

public class SimulationState
{
    private object _IsRunningLock = new();
    public DateTime CurrentTime;
    public bool IsRunning;
    public List<Bus> Buses { get; private set; } = new();
    public List<Station> Stations { get; private set; } = new();
    public double TotalRouteLengthKm { get; set; }
    public SimulationConfig? ConfigSnapshot { get; set; }
}