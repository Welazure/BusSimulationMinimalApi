using System.Text.Json;
using BusSimulationMinimal.Services.Configuration.Interface;
using BusSimulationMinimal.Services.Simulation.Interface;
using BusSimulationMinimal.Services.Simulation.States;

namespace BusSimulationMinimal.Services.Simulation;

public class Orchestrator : IOrchestrator
{
    private readonly object _lock = new();
    private readonly IBusService _busService;
    private readonly IConfigurationService _configurationService;
    private readonly IPassengerService _passengerService;
    private SimulationState? _state;
    private readonly IStationService _stationService;

    public Orchestrator(
        IBusService busService,
        IStationService stationService,
        IPassengerService passengerService,
        IConfigurationService configurationService)
    {
        _busService = busService;
        _stationService = stationService;
        _passengerService = passengerService;
        _configurationService = configurationService;
    }

    public void PerformSimulationTick()
    {
        lock (_lock)
        {
            if (_state == null || !_state.IsRunning) return;

            _state.CurrentTime = _state.CurrentTime.AddSeconds(1);
            _busService.UpdateStateAndPositions(_state, _configurationService.RouteConf,
                _configurationService.SimulationConf);
            _passengerService.GeneratePassengers(_state, _configurationService.PassengerConf,
                _configurationService.SimulationConf);
            _stationService.ProcessStationInteractions(_state, _configurationService.RouteConf,
                _configurationService.SimulationConf);
        }
    }

    public string GetStateAsJson()
    {
        return GetState();
    }

    public void StartSimulation()
    {
        lock (_lock)
        {
            if (_state == null) InitializeState();
            _state.IsRunning = true;
        }
    }

    public void PauseSimulation()
    {
        lock (_lock)
        {
            if (_state != null) _state.IsRunning = false;
        }
    }

    public void StopSimulation()
    {
        lock (_lock)
        {
            _state = null;
        }
    }

    public void ResetSimulation()
    {
        InitializeState();
    }

    public void DispatchBus(int? capacityOverride = null, string? startAtStationIdOverride = null)
    {
        lock (_lock)
        {
            var routeConfig = _configurationService.RouteConf;
            var simulationConfig = _configurationService.SimulationConf;
            _busService.DispatchBus(_state, simulationConfig, routeConfig, capacityOverride, startAtStationIdOverride);
        }
    }

    public bool InstructBusToReturnToPool(Guid busId)
    {
        lock (_lock)
        {
            if (_state == null) return false;
            return _busService.ReturnBusToPool(_state, busId);
        }
    }

    public bool InstructBusToMoveFromPool(Guid busId)
    {
        lock (_lock)
        {
            if (_state == null) return false;
            return _busService.MoveBusFromPool(_state, busId);
        }
    }

    public bool DestructBus(Guid busId)
    {
        lock (_lock)
        {
            if (_state == null) return false;
            return _busService.DestructBus(_state, busId);
        }
    }

    public string GetState()
    {
        lock (_lock)
        {
            if (_state == null) return "{ }";

            return JsonSerializer.Serialize<SimulationState>(_state, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }

    private void InitializeState()
    {
        lock (_lock)
        {
            _state = new SimulationState
            {
                // TODO
            };
        }
    }
}