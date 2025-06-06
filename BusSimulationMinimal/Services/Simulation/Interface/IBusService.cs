﻿using BusSimulationMinimal.Services.Configuration.Typing;
using BusSimulationMinimal.Services.Simulation.States;

namespace BusSimulationMinimal.Services.Simulation.Interface;

public interface IBusService
{
    public void UpdateStateAndPositions(SimulationState state, RouteConfig routeConfig,
        SimulationConfig simulationConfig);

    public void DispatchBus(SimulationState state, SimulationConfig simulationConfig, RouteConfig routeConfig,
        int? capacityOverride,
        string? startAtStationIdOverride);

    public string[] GetNextStationIdsNoLoop(SimulationState state, string currentStationId, bool reversed = false);
    public string GetNextStationId(SimulationState state, string currentStationId, bool? includePool, bool reversed = false);
    public bool ReturnBusToPool(SimulationState state, Guid busId);
    public bool MoveBusFromPool(SimulationState state, Guid busId);
    bool DestructBus(SimulationState state, Guid busId);
    bool GetNextDirection(SimulationState state, string currentStationId, string nextStationId);
}