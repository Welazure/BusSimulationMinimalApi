using System.Text.Json;
using BusSimulationMinimal.Services.Configuration.Interface;
using BusSimulationMinimal.Services.Simulation.Interface;
using BusSimulationMinimal.Services.Simulation.States;
using BusSimulationMinimal.Services.Simulation.Typing;

// For SimulationState

// For Bus, Station, Passenger models

namespace BusSimulationMinimal.Services.Simulation;

public class NewOrchestrator : IOrchestrator
{
    private readonly IBusService _busService;
    private readonly IConfigurationService _configurationService;
    private readonly IPassengerService _passengerService;

    private readonly object _simulationWriteLock = new();
    private readonly object _snapshotPointerLock = new();
    private readonly IStationService _stationService;
    private SimulationState? _liveSimulationState;
    private SimulationState? _readableSnapshotState; // This will hold the deep copy for readers


    public NewOrchestrator(
        IBusService busService,
        IStationService stationService,
        IPassengerService passengerService,
        IConfigurationService configurationService)
    {
        _busService = busService;
        _stationService = stationService;
        _passengerService = passengerService;
        _configurationService = configurationService;

        // Initialize the simulation state immediately.
        // This ensures _liveSimulationState and _readableSnapshotState are never null after construction,
        // simplifying null checks in other methods.
        InitializeSimulationStateAndFirstSnapshot();
    }

    public void PerformSimulationTick() // Called by IHostedService
    {
        lock (_simulationWriteLock)
        {
            PerformSimulationTickInternal();
        }
    }

    public string GetStateAsJson() // Changed from GetState() to return JSON string directly
    {
        SimulationState? snapshotToSerialize;
        lock (_snapshotPointerLock) // Brief lock to get the current snapshot reference
        {
            snapshotToSerialize = _readableSnapshotState;
        }

        if (snapshotToSerialize == null) return "{}";

        return JsonSerializer.Serialize(snapshotToSerialize,
            new JsonSerializerOptions
            {
                WriteIndented = true, // For better readability in JSON output
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Use camelCase for JSON properties
            });
    }

    public void StartSimulation()
    {
        lock (_simulationWriteLock)
        {
            if (_liveSimulationState == null)
                InitializeSimulationStateAndFirstSnapshot();
            if (_liveSimulationState != null)
            {
                _liveSimulationState.IsRunning = true;
                UpdateReadableSnapshotInternal();
            }

            else
            {
                throw new Exception("Simulation not initialized.");
            }
        }
    }

    public void PauseSimulation()
    {
        lock (_simulationWriteLock)
        {
            if (_liveSimulationState != null)
            {
                _liveSimulationState.IsRunning = false;
                UpdateReadableSnapshotInternal();
            }
        }
    }

    public void StopSimulation()
    {
        lock (_simulationWriteLock)
        {
            _liveSimulationState = null;
            lock (_snapshotPointerLock)
            {
                _readableSnapshotState = null;
            }
        }
    }

    public void ResetSimulation()
    {
        lock (_simulationWriteLock) // Ensure entire reset is atomic
        {
            // Re-initialize both live and snapshot states
            InitializeSimulationStateAndFirstSnapshot(_liveSimulationState.CurrentTime);
            // IsRunning will be false by default from InitializeSimulationStateAndFirstSnapshot
        }
    }

    public void DispatchBus(int? capacityOverride = null, string? startAtStationIdOverride = null)
    {
        lock (_simulationWriteLock)
        {
            if (_liveSimulationState == null) throw new InvalidOperationException("Simulation not initialized.");

            _busService.DispatchBus(_liveSimulationState, _configurationService.SimulationConf,
                _configurationService.RouteConf, capacityOverride, startAtStationIdOverride);
            UpdateReadableSnapshotInternal(); // Reflect new bus in snapshot
        }
    }

    public bool InstructBusToReturnToPool(Guid busId)
    {
        var result = false;
        lock (_simulationWriteLock)
        {
            if (_liveSimulationState == null) return false;
            result = _busService.ReturnBusToPool(_liveSimulationState, busId);
            if (result) // Only update snapshot if state actually changed
                UpdateReadableSnapshotInternal();
        }

        return result;
    }

    public bool InstructBusToMoveFromPool(Guid busId)
    {
        var result = false;
        lock (_simulationWriteLock)
        {
            if (_liveSimulationState == null) return false;
            result = _busService.MoveBusFromPool(_liveSimulationState, busId);
            if (result) UpdateReadableSnapshotInternal();
        }

        return result;
    }

    public bool DestructBus(Guid busId)
    {
        var result = false;
        lock (_simulationWriteLock) // Ensure exclusive access to modify _liveSimulationState
        {
            if (_liveSimulationState == null)
                // Log error: Simulation not initialized
                return false;

            result = _busService.DestructBus(_liveSimulationState, busId);

            if (result) // Only update snapshot if a change actually occurred
                UpdateReadableSnapshotInternal(); // Reflect the bus removal in the snapshot
        }

        return result;
    }

    public void GeneratePassengersOnStations(string id, int count)
    {
        lock (_simulationWriteLock) // Ensure thread-safe access to _liveSimulationState
        {
            if (_liveSimulationState == null)
                throw new InvalidOperationException("Simulation not initialized.");

            _passengerService.GeneratePassengersOnStation(_liveSimulationState, _configurationService.PassengerConf, _configurationService.SimulationConf, id, count);
            UpdateReadableSnapshotInternal(); // Update snapshot after generating passengers
        }
    }

    private void InitializeSimulationStateAndFirstSnapshot(DateTime? startTime = null)
    {
        // This method is called only from the constructor, so no external lock needed here.
        // However, modifications to _liveSimulationState and _readableSnapshotState need to be safe.
        // Since it's constructor, it's inherently thread-safe for initialization.

        var routeConfig = _configurationService.RouteConf; // Assuming config is loaded and ready
        var initialStations = routeConfig.Stations.Select(sc => new Station
        {
            Id = sc.Id,
            Name = sc.Name,
            PositionOnRouteKm = sc.PositionOnRouteKm,
            CurrentBusId = null,
            WaitingBuses = new List<Guid>(),
            WaitingPassengers = new List<Passenger>()
        }).ToList();

        _liveSimulationState = new SimulationState
        {
            Stations = initialStations,
            Buses = new List<Bus>(),
            CurrentTime = startTime ?? DateTime.Now, // Default start time
            IsRunning = false,
            TotalRouteLengthKm = routeConfig.TotalRouteLengthKm
        };
        _readableSnapshotState = _liveSimulationState.DeepClone();
    }


    private void PerformSimulationTickInternal()
    {
        if (_liveSimulationState == null || !_liveSimulationState.IsRunning) return;

        _liveSimulationState.CurrentTime = _liveSimulationState.CurrentTime.AddSeconds(1);
        _busService.UpdateStateAndPositions(_liveSimulationState, _configurationService.RouteConf,
            _configurationService.SimulationConf);
        _passengerService.GeneratePassengers(_liveSimulationState, _configurationService.PassengerConf,
            _configurationService.SimulationConf);
        _stationService.ProcessStationInteractions(_liveSimulationState, _configurationService.RouteConf,
            _configurationService.SimulationConf); // Pass RouteConfig


        UpdateReadableSnapshotInternal();
    }


    private void UpdateReadableSnapshotInternal()
    {
        if (_liveSimulationState == null) return;

        var newSnapshot = _liveSimulationState.DeepClone();
        lock (_snapshotPointerLock)
        {
            _readableSnapshotState = newSnapshot;
        }
    }
}