using System.Text.Json;
using BusSimulationMinimal.Services.Configuration.Interface;
using BusSimulationMinimal.Services.Simulation.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BusSimulationMinimal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;
    private readonly IOrchestrator _orchestrator;

    public SimulationController(IOrchestrator orchestrator, IConfigurationService configurationService)
    {
        _orchestrator = orchestrator;
        _configurationService = configurationService;
    }

    [HttpGet("state")]
    public string GetState()
    {
        return _orchestrator.GetStateAsJson();
    }

    [HttpPost("start")]
    public IActionResult StartSimulation()
    {
        _orchestrator.StartSimulation();
        return Ok("Simulation started.");
    }

    [HttpPost("pause")]
    public IActionResult PauseSimulation()
    {
        _orchestrator.PauseSimulation();
        return Ok("Simulation paused.");
    }

    [HttpPost("stop")]
    public IActionResult StopSimulation()
    {
        _orchestrator.StopSimulation();
        return Ok("Simulation stopped.");
    }

    [HttpPost("reset")]
    public IActionResult ResetSimulation()
    {
        _orchestrator.ResetSimulation();
        return Ok("Simulation reset.");
    }

    [HttpGet("config")]
    public IActionResult GetConfig()
    {
        // Assuming you have a method to get the configuration as JSON
        var config = JsonSerializer.Serialize(_configurationService);
        return Ok(config);
    }
}