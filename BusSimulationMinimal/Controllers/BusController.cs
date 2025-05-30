using BusSimulationMinimal.Services.Simulation.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BusSimulationMinimal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusController : ControllerBase
{
    private readonly IOrchestrator _orchestrator;

    public BusController(IOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    [HttpPost("dispatch")]
    public IActionResult DispatchBus([FromQuery] int? capacityOverride = null,
        [FromQuery] string? startAtStationIdOverride = null)
    {
        _orchestrator.DispatchBus(capacityOverride, startAtStationIdOverride);
        return Created("/api/bus", "Bus dispatched");
    }

    [HttpPost("{id:guid}/returnToPool")]
    public IActionResult ReturnBusToPool(Guid id)
    {
        if (_orchestrator.InstructBusToReturnToPool(id)) return Ok("Bus instructed to return to pool");
        return NotFound("Bus not found or not in a state to return to pool");
    }

    [HttpPost("{id:guid}/moveFromPool")]
    public IActionResult MoveBusFromPool(Guid id)
    {
        if (_orchestrator.InstructBusToMoveFromPool(id)) return Ok("Bus moved from pool");
        return NotFound("Bus not found or not in a state to move from pool");
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DestructBus(Guid id)
    {
        if (_orchestrator.DestructBus(id)) return Ok("Bus destructed");
        return NotFound("Bus not found or not in a state to be destructed");
    }
}