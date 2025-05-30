using BusSimulationMinimal.Services.Configuration.Typing;
using BusSimulationMinimal.Services.Simulation.Interface;
using BusSimulationMinimal.Services.Simulation.States;
using BusSimulationMinimal.Services.Simulation.Typing;

namespace BusSimulationMinimal.Services.Simulation;

public class BusService : IBusService
{
    public void UpdateStateAndPositions(SimulationState state, RouteConfig routeConfig,
        SimulationConfig simulationConfig)
    {
        foreach (var bus in state.Buses)
        {
            if (bus.Status != BusStatus.MOVING) continue;
            if (string.IsNullOrEmpty(bus.NextStationId)) continue;
            var distanceThisTick = simulationConfig.BusSpeedKmh / 3600.0;
            var previousPosition = bus.PositionOnRouteKm; // Store for precise arrival detection
            bus.PositionOnRouteKm = (bus.PositionOnRouteKm + distanceThisTick) % routeConfig.TotalRouteLengthKm;

            var nextStation = state.Stations.FirstOrDefault(x => x.Id == bus.NextStationId);
            if (nextStation == null) continue;
            var arrived = false;
            if (previousPosition < nextStation.PositionOnRouteKm &&
                bus.PositionOnRouteKm >= nextStation.PositionOnRouteKm)
                arrived = true;
            else if (previousPosition > bus.PositionOnRouteKm) // Bus wrapped around the route
                if (bus.PositionOnRouteKm >= nextStation.PositionOnRouteKm ||
                    previousPosition < nextStation.PositionOnRouteKm) // Station is after wrap or before wrap
                    arrived = true;

            if (arrived)
            {
                bus.PositionOnRouteKm = nextStation.PositionOnRouteKm; // Snap to station

                if (nextStation.CurrentBusId == null)
                {
                    nextStation.CurrentBusId = bus.Id;
                    bus.Status = BusStatus.AT_STATION;
                    bus.TimeSpentAtStationSec = 0;
                    bus.CurrentStationId = nextStation.Id;
                    bus.NextStationId = GetNextStationId(state, nextStation.Id); // <-- SET NEXT TARGET
                }
                else
                {
                    if (!nextStation.WaitingBuses.Contains(bus.Id)) nextStation.WaitingBuses.Add(bus.Id);
                    bus.Status = BusStatus.WAITING_FOR_STATION_EMPTY;
                }
            }
        }
    }

    public string GetNextStationId(SimulationState state, string currentStationId)
    {
        var stationIds = state.Stations.Select(x => x.Id).ToList();
        var currentIndex = stationIds.IndexOf(currentStationId);
        if (currentIndex == -1 || stationIds.Count == 0)
            throw new ArgumentException("Current station ID not found in the list of stations.");

        var nextIndex = (currentIndex + 1) % stationIds.Count; // Wrap around to the first station
        return stationIds[nextIndex];
    }

    public void DispatchBus(SimulationState state, SimulationConfig simulationConfig, RouteConfig routeConfig,
        int? capacityOverride,
        string? startAtStationIdOverride)
    {
        var bus = new Bus();
        bus.Id = Guid.NewGuid();
        bus.Capacity = capacityOverride ?? simulationConfig.DefaultBusCapacity;
        var startStation = state.Stations
            .FirstOrDefault(x => x.Id == (startAtStationIdOverride ?? "POOL"));
        if (startStation == null) throw new ArgumentException("Start station not found in route configuration.");

        if (startStation.CurrentBusId != null)
        {
            if (!startStation.WaitingBuses.Contains(bus.Id))
                startStation.WaitingBuses.Add(bus.Id);
            bus.Status = BusStatus.WAITING_FOR_STATION_EMPTY;
            bus.NextStationId = startStation.Id;
        }
        else
        {
            startStation.CurrentBusId = bus.Id;
            bus.Status = BusStatus.AT_STATION;
            bus.TimeSpentAtStationSec = 0;
            bus.CurrentStationId = startStation.Id;
            bus.NextStationId = GetNextStationId(state, startStation.Id);
        }

        bus.PositionOnRouteKm = startStation.PositionOnRouteKm;
        state.Buses.Add(bus);
    }

    public bool ReturnBusToPool(SimulationState state, Guid busId)
    {
        var bus = state.Buses.FirstOrDefault(b => b.Id == busId);
        if (bus == null || bus.Status == BusStatus.INACTIVE) return false;

        bus.ReturningToPool = true;
        return true;
    }

    public bool MoveBusFromPool(SimulationState state, Guid busId)
    {
        var bus = state.Buses.FirstOrDefault(b => b.Id == busId);
        if (bus == null || bus.Status != BusStatus.INACTIVE || bus.CurrentStationId != "POOL") return false;

        bus.Status = BusStatus.AT_STATION;
        bus.PositionOnRouteKm = state.Stations.FirstOrDefault(x => x.Id == "POOL")?.PositionOnRouteKm ?? 0;
        bus.NextStationId = GetNextStationId(state, bus.CurrentStationId);
        return true;
    }

    public bool DestructBus(SimulationState state, Guid busId)
    {
        var bus = state.Buses.FirstOrDefault(b => b.Id == busId);
        if (bus == null) return false;

        // Remove the bus from the state
        state.Buses.Remove(bus);

        // Remove the bus from any station's current or waiting lists
        foreach (var station in state.Stations)
        {
            if (station.CurrentBusId == busId)
                station.CurrentBusId = null;
            station.WaitingBuses.Remove(busId);
        }

        return true;
    }
}