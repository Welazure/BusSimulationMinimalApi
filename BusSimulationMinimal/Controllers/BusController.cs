using BusSimulationMinimal.Models;
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
    public IActionResult DispatchBus([FromBody] DispatchBusRequestDto dispatchRequest)
    {
        if (dispatchRequest == null)
        {
            return BadRequest("Dispatch request data is missing.");
        }


        bool reversed = false; // Default to forward
        if (!string.IsNullOrEmpty(dispatchRequest.Direction))
        {
            if (dispatchRequest.Direction.Equals("BACKWARD", StringComparison.OrdinalIgnoreCase))
            {
                reversed = true;
            }
        }
        _orchestrator.DispatchBus(
            dispatchRequest.Capacity,
            dispatchRequest.StartAtStationId
        );
            return Ok(new { message = "Bus dispatch command issued." });
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