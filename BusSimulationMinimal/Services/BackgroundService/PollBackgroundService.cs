using BusSimulationMinimal.Services.Configuration.Interface;
using BusSimulationMinimal.Services.Simulation.Interface;

namespace BusSimulationMinimal.Services.Simulation;

public class PollBackgroundService : IHostedService, IDisposable
{
    private readonly IConfigurationService _config;
    private readonly ILogger<SimulationBackgroundService> _logger;
    private Timer? _timer;
    private static readonly HttpClient client = new HttpClient();
    public PollBackgroundService(ILogger<SimulationBackgroundService> logger,
        IConfigurationService config)
    {
        _logger = logger;
        _config = config;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _logger.LogInformation("SimulationBackgroundService disposed.");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{serviceName} is starting.", nameof(SimulationBackgroundService));
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromMilliseconds(1000));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        // _logger.LogInformation("SimulationBackgroundService is working.");
        try
        {
            var response = client.PostAsync("http://localhost:3000/api/log_snapshot", new StringContent("")).Result;
            //var response2 = client.PostAsync("http://localhost:3000/api/dispatch", new StringContent("")).Result;
            Console.WriteLine("tick performed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during simulation tick.");
        }
    }
}