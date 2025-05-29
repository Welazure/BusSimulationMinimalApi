using BusSimulationMinimal.Services.Configuration.Typing;

namespace BusSimulationMinimal.Services.Configuration.Interface;

public interface IConfigurationService
{
    PassengerConfig? PassengerConf { get; set; }
    RouteConfig? RouteConf { get; set; }
    SimulationConfig? SimulationConf { get; set; }

    void CheckFilesExists(bool copyFiles = true);
    string GetBasePath();
    void SavePassengerConfig();
    void SaveRouteConfig();
    void SaveSimulationConfig();
    void SaveAllConfigs();
    void LoadAllConfigs();
    void LoadPassengerConfig();
    void LoadRouteConfig();
    void LoadSimulationConfig();
}