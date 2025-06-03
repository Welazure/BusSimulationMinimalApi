namespace BusSimulationMinimal.Services.Simulation.Interface;

public interface IOrchestrator
{
    void PerformSimulationTick();
    string GetStateAsJson();
    void StartSimulation();
    void PauseSimulation();
    void StopSimulation();
    void ResetSimulation();
    void DispatchBus(int? capacityOverride = null, string? startAtStationIdOverride = null);
    bool InstructBusToReturnToPool(Guid busId);
    bool InstructBusToMoveFromPool(Guid busId);
    bool DestructBus(Guid busId);
    
    void GeneratePassengersOnStations(string id, int count);
}