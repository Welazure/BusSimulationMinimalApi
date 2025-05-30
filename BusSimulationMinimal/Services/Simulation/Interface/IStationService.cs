using BusSimulationMinimal.Services.Configuration.Typing;
using BusSimulationMinimal.Services.Simulation.States;

namespace BusSimulationMinimal.Services.Simulation.Interface;

public interface IStationService
{
    void ProcessStationInteractions(SimulationState state, RouteConfig routeConfig, SimulationConfig simulationConfig);
}