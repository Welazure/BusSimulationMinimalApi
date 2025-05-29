using System.Text.Json;
using BusSimulationMinimal.Services.Configuration.Interface;
using BusSimulationMinimal.Services.Configuration.Typing;

namespace BusSimulationMinimal.Services.Configuration;

public class ConfigurationService : IConfigurationService
{
    private static readonly string BasePath = "./config/";

    private PassengerConfig? _passengerConfig;
    private RouteConfig? _routeConfig;
    private SimulationConfig? _simulationConfig;

    public ConfigurationService()
    {
        CheckFilesExists();
        LoadAllConfigs();
    }

    public void CheckFilesExists(bool copyFiles = true)
    {
        var files = new[] { "routeConfig.json", "simulationConfig.json", "passengerConfig.json" };
        if (!Directory.Exists(BasePath)) Directory.CreateDirectory(BasePath);

        foreach (var file in files)
            if (!File.Exists(BasePath + file))
            {
                if (copyFiles)
                {
                    Console.WriteLine("File " + file + " does not exist! Copying..");
                    File.Copy("./Defaults/" + file, BasePath + file);
                    Console.WriteLine("Copied!");
                }
                else
                {
                    Console.WriteLine("File " + file + " does not exist! Please copy it from the Defaults folder.");
                }
            }
    }

    public string GetBasePath()
    {
        return BasePath;
    }

    public PassengerConfig? getPassengerConfig()
    {
        return _passengerConfig;
    }

    public RouteConfig? getRouteConfig()
    {
        return _routeConfig;
    }

    public SimulationConfig? getSimulationConfig()
    {
        return _simulationConfig;
    }

    public void SetPassengerConfig(PassengerConfig passengerConfig)
    {
        _passengerConfig = passengerConfig;
        SavePassengerConfig();
    }

    public void SetRouteConfig(RouteConfig routeConfig)
    {
        _routeConfig = routeConfig;
        SaveRouteConfig();
    }

    public void SetSimulationConfig(SimulationConfig simulationConfig)
    {
        _simulationConfig = simulationConfig;
        SaveSimulationConfig();
    }

    public void SavePassengerConfig()
    {
        if (_passengerConfig == null) throw new InvalidOperationException("PassengerConfig is not set.");

        var filePath = Path.Combine(BasePath, "passengerConfig.json");
        File.WriteAllText(filePath, JsonSerializer.Serialize(_passengerConfig));
    }

    public void SaveRouteConfig()
    {
        if (_routeConfig == null) throw new InvalidOperationException("RouteConfig is not set.");

        var filePath = Path.Combine(BasePath, "routeConfig.json");
        File.WriteAllBytes(filePath, JsonSerializer.SerializeToUtf8Bytes(_routeConfig));
    }

    public void SaveSimulationConfig()
    {
        if (_simulationConfig == null) throw new InvalidOperationException("SimulationConfig is not set.");

        var filePath = Path.Combine(BasePath, "simulationConfig.json");
        File.WriteAllBytes(filePath, JsonSerializer.SerializeToUtf8Bytes(_simulationConfig));
    }

    public void SaveAllConfigs()
    {
        SavePassengerConfig();
        SaveRouteConfig();
        SaveSimulationConfig();
    }

    public void LoadAllConfigs()
    {
        LoadPassengerConfig();
        LoadRouteConfig();
        LoadSimulationConfig();
    }

    public void LoadPassengerConfig()
    {
        var filePath = Path.Combine(BasePath, "passengerConfig.json");
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            _passengerConfig = JsonSerializer.Deserialize<PassengerConfig>(json);
        }
        else
        {
            throw new FileNotFoundException("PassengerConfig file not found.", filePath);
        }
    }

    public void LoadRouteConfig()
    {
        var filePath = Path.Combine(BasePath, "routeConfig.json");
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            _routeConfig = JsonSerializer.Deserialize<RouteConfig>(json);
        }
        else
        {
            throw new FileNotFoundException("RouteConfig file not found.", filePath);
        }
    }

    public void LoadSimulationConfig()
    {
        var filePath = Path.Combine(BasePath, "simulationConfig.json");
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            _simulationConfig = JsonSerializer.Deserialize<SimulationConfig>(json);
        }
        else
        {
            throw new FileNotFoundException("SimulationConfig file not found.", filePath);
        }
    }
}