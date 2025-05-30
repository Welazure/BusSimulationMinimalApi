using BusSimulationMinimal.Services.Configuration.Typing;
using BusSimulationMinimal.Services.Simulation.Interface;
using BusSimulationMinimal.Services.Simulation.States;
using BusSimulationMinimal.Services.Simulation.Typing;

namespace BusSimulationMinimal.Services.Simulation;

public class StationService : IStationService
{
    private readonly IBusService _busService;

    public StationService(IBusService busService)
    {
        _busService = busService;
    }

    public void ProcessStationInteractions(SimulationState state, RouteConfig routeConfig,
        SimulationConfig simulationConfig)
    {
        foreach (var currStation in state.Stations)
            if (currStation.CurrentBusId != null)
            {
                var bus = state.Buses.FirstOrDefault(x => x.Id == currStation.CurrentBusId);
                if (bus == null || bus.Status != BusStatus.AT_STATION || bus.CurrentStationId != currStation.Id)
                {
                    bus.Status = BusStatus.MOVING;
                    bus.CurrentStationId = null;
                    bus.TimeSpentAtStationSec = 0;
                    currStation.CurrentBusId = null; // Free the station platfo
                    bus.NextStationId = _busService.GetNextStationId(state, currStation.Id);
                    continue;
                }

                // Bus is valid, calculate.
                if (bus.TimeSpentAtStationSec >= simulationConfig.MaximumBusWaitTimeSeconds ||
                    (bus.Passengers.Count >= bus.Capacity &&
                     bus.Passengers.Count(x => x.DestinationStationId == currStation.Id) < 0))
                {
                    bus.Status = BusStatus.MOVING;
                    bus.CurrentStationId = null;
                    bus.TimeSpentAtStationSec = 0;
                    currStation.CurrentBusId = null; // Free the station platform
                    continue;
                }

                // Time has not exceeded maximum wait time, continue processing
                bus.TimeSpentAtStationSec += 1.0; // Increment time spent at station

                // Calculate processing ratio
                int alightingCount, boardingCount, totalToProcess = simulationConfig.PassengerInteractionPerSecond;

                if (bus.ReturningToPool)
                {
                    alightingCount = totalToProcess;
                    boardingCount = 0;
                }
                else
                {
                    alightingCount = Random.Shared.Next(0, totalToProcess + 1);
                    boardingCount = totalToProcess - alightingCount;
                }

                // Process Alighting
                var toBeAlighted =
                    bus.Passengers.Where(x => x.DestinationStationId == currStation.Id).ToList();
                for (var i = 0; i < alightingCount; i++)
                {
                    if (toBeAlighted.Count < 1) break;
                    var passenger = toBeAlighted[Random.Shared.Next(0, toBeAlighted.Count)];
                    passenger.Status = PassengerStatus.ALIGHTED;
                    bus.Passengers.Remove(passenger);
                }

                var eligibleToBoard = currStation.WaitingPassengers.Where(x => x.Status == PassengerStatus.WAITING)
                    .ToList();
                for (var i = 0; i < boardingCount; i++)
                {
                    if (bus.Passengers.Count >= bus.Capacity) break;
                    if (eligibleToBoard.Count < 1) break;
                    var passenger =
                        eligibleToBoard[
                            Random.Shared.Next(0, eligibleToBoard.Count)];
                    passenger.Status = PassengerStatus.ON_BUS;
                    bus.Passengers.Add(passenger);
                    currStation.WaitingPassengers.Remove(passenger);
                    eligibleToBoard.Remove(passenger); // Remove from the temporary list for this tick's processing
                }

                if (!currStation.WaitingPassengers.Any(x => x.Status == PassengerStatus.WAITING) &&
                    !bus.Passengers.Any(x => x.DestinationStationId == currStation.Id))
                {
                    bus.Status = BusStatus.MOVING;
                    bus.CurrentStationId = null;
                    bus.TimeSpentAtStationSec = 0;
                    currStation.CurrentBusId = null; // Free the station platform
                }
            }
            else if (currStation.WaitingBuses.Count > 0)
            {
                // A bus is waiting for the station to be free
                var waitingBus = state.Buses.FirstOrDefault(x => x.Id == currStation.WaitingBuses[0] &&
                                                                 x.Status == BusStatus.WAITING_FOR_STATION_EMPTY);

                if (waitingBus != null)
                {
                    // Platform is now free, bus docks
                    currStation.CurrentBusId = waitingBus.Id;
                    waitingBus.Status = BusStatus.AT_STATION;
                    waitingBus.TimeSpentAtStationSec = 0;
                    waitingBus.CurrentStationId = currStation.Id;
                    currStation.WaitingBuses.Remove(waitingBus.Id);
                    waitingBus.NextStationId = _busService.GetNextStationId(state, currStation.Id);
                }
            }
    }
}