using BusSimulationMinimal.Services.Configuration.Typing;
using BusSimulationMinimal.Services.Simulation.Interface;
using BusSimulationMinimal.Services.Simulation.States;
using BusSimulationMinimal.Services.Simulation.Typing;

namespace BusSimulationMinimal.Services.Simulation;

public class PassengerService : IPassengerService
{
    public void GeneratePassengers(SimulationState state, PassengerConfig passengerConfig,
        SimulationConfig simulationConfig)
    {
        if (state.CurrentTime.Second % Random.Shared.Next(2, 6) != 0)
            return;
        var passengerGenerationMultiplier = GetCurrentGenerationMultiplier(
            state.CurrentTime.TimeOfDay, passengerConfig.generationMultiplierGraph);
        foreach (var station in state.Stations)
        {
            if (station.Name == "POOL")
                continue;
            var baseNum = passengerConfig.basePassengerSpawnRate * passengerGenerationMultiplier;
            var jitter = (Random.Shared.NextDouble() * 2 - 1) * simulationConfig.PassengerSpawnJitterFactor *
                         baseNum;
            var numToSpawn = (int)Math.Max(0, Math.Round(baseNum + jitter));

            List<DestinationWeightPoint> destinationWeightPoints = new();
            int totalWeight = 0;
            if (passengerConfig.DestinationWeights.TryGetValue(station.Id, out var destinationsFromThisOrigin))
            {
                if (destinationsFromThisOrigin.Count != 0)
                {
                    // This sum should ideally be pre-calculated when config is loaded, per origin.
                    int totalWeightForThisOrigin = destinationsFromThisOrigin.Sum(dw => dw.Weight);
                    if (totalWeightForThisOrigin > 0)
                    {
                        // Weights are already normalized to 100, no need to adjust.
                        destinationWeightPoints = destinationsFromThisOrigin;
                        totalWeight = totalWeightForThisOrigin;
                    }
                    else
                    {
                        throw new ArgumentException("Weights for destination weights cannot be zero or negative.");
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    $"No destination weights configured for station {station.Id} ({station.Name}).");
            }

            for (var i = 0; i < numToSpawn; i++)
            {
                var passenger = new Passenger();
                passenger.Id = Guid.NewGuid();
                passenger.OriginStationId = station.Id;
                passenger.Status = PassengerStatus.WAITING;

                var weight = Random.Shared.Next(0, totalWeight);
                var destinationStationId = destinationWeightPoints.FirstOrDefault(w => (weight -= w.Weight) < 0)?.DestinationStationId;
                passenger.DestinationStationId = destinationStationId ?? string.Empty;
                if(!string.IsNullOrEmpty(destinationStationId))
                    station.WaitingPassengers.Add(passenger);
            }
        }
    }

    /// <summary>
    ///     Calculates the current passenger generation multiplier based on the time of day
    ///     by linearly interpolating values from the provided graph.
    /// </summary>
    /// <param name="currentTimeOfDay">The current time of day in the simulation.</param>
    /// <param name="sortedMultiplierGraph">
    ///     A list of TimeMultiplierPoint objects,
    ///     pre-sorted by TimeOfDay.
    /// </param>
    /// <returns>The calculated multiplier.</returns>
    public double GetCurrentGenerationMultiplier(TimeSpan currentTimeOfDay,
        List<TimeMultiplierPoint> sortedMultiplierGraph)
    {
        if (!sortedMultiplierGraph.Any())
            // Default multiplier if the graph is empty or not provided
            // Log a warning here if this is unexpected
            return 1.0;

        var graph = sortedMultiplierGraph;


        // Case 1: Graph has only one point
        if (graph.Count == 1) return graph[0].Multiplier;

        // Case 2: Current time is before the first point in the graph
        if (currentTimeOfDay <= graph[0].TimeOfDay) return graph[0].Multiplier;

        // Case 3: Current time is after the last point in the graph
        if (currentTimeOfDay >= graph[^1].TimeOfDay) return graph[^1].Multiplier;

        // Case 4: Current time is between two points in the graph; find these points
        TimeMultiplierPoint? point1 = null;
        TimeMultiplierPoint? point2 = null;

        for (var i = 0; i < graph.Count - 1; i++)
            if (currentTimeOfDay >= graph[i].TimeOfDay && currentTimeOfDay <= graph[i + 1].TimeOfDay)
            {
                point1 = graph[i];
                point2 = graph[i + 1];
                break;
            }

        if (point1 == null || point2 == null)
            // This should not happen if the previous checks are correct and graph is sorted.
            // Fallback to the closest point or a default.
            // Log an error here.
            return 1;

        // If current time exactly matches one of the points
        if (currentTimeOfDay == point1.TimeOfDay) return point1.Multiplier;

        if (currentTimeOfDay == point2.TimeOfDay) return point2.Multiplier;

        // Perform linear interpolation
        // M = M1 + (t - T1) * (M2 - M1) / (T2 - T1)
        // where M is the interpolated multiplier, T is time.

        var t1Seconds = point1.TimeOfDay.TotalSeconds;
        var t2Seconds = point2.TimeOfDay.TotalSeconds;
        var currentTimeSeconds = currentTimeOfDay.TotalSeconds;

        var m1 = point1.Multiplier;
        var m2 = point2.Multiplier;

        // Avoid division by zero if times are identical (shouldn't happen with distinct sorted points)
        if (Math.Abs(t2Seconds - t1Seconds) < 0.0001) return m1; // Or (m1 + m2) / 2, or m2

        var interpolatedMultiplier = m1 + (currentTimeSeconds - t1Seconds) * (m2 - m1) / (t2Seconds - t1Seconds);

        return interpolatedMultiplier;
    }
}