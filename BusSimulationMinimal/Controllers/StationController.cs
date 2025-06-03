using BusSimulationMinimal.Services.Simulation.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BusSimulationMinimal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StationController : ControllerBase
{
    private readonly IOrchestrator _orchestrator;
    private readonly IPassengerService _passengerService;

    public StationController(IOrchestrator orchestrator, IPassengerService passengerService)
    {
        _orchestrator = orchestrator;
        _passengerService = passengerService;
    }
    [HttpPost("{id}/generatePassengers/{count}")]
    public void GeneratePassengers(string id, int count)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero.", nameof(count));
        }

        _orchestrator.GeneratePassengersOnStations(id, count);
    }
}