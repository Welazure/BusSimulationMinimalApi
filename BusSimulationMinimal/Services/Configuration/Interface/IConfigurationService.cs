using BusSimulationMinimal.Services.Configuration.Typing;

namespace BusSimulationMinimal.Services.Configuration.Interface;

public interface IConfigurationService
{
    void CheckFilesExists(bool copyFiles = true);
    string GetBasePath();
    PassengerConfig? getPassengerConfig();
    RouteConfig? getRouteConfig();
    SimulationConfig? getSimulationConfig();
    void SetPassengerConfig(PassengerConfig passengerConfig);
    void SetRouteConfig(RouteConfig routeConfig);
    void SetSimulationConfig(SimulationConfig simulationConfig);
    void SavePassengerConfig();
    void SaveRouteConfig();
    void SaveSimulationConfig();
    void SaveAllConfigs();
    void LoadAllConfigs();
    void LoadPassengerConfig();
    void LoadRouteConfig();
    void LoadSimulationConfig();
}