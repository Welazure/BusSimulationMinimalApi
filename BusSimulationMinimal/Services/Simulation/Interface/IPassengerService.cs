using BusSimulationMinimal.Services.Configuration.Typing;
using BusSimulationMinimal.Services.Simulation.States;

namespace BusSimulationMinimal.Services.Simulation.Interface;

public interface IPassengerService
{
    public void GeneratePassengers(SimulationState state, PassengerConfig passengerConfig,
        SimulationConfig simulationConfig);

    public void GeneratePassengersOnStation(SimulationState state, PassengerConfig passengerConfig,
        SimulationConfig simulationConfig, string stationId,
        int? numToSpawn);
}