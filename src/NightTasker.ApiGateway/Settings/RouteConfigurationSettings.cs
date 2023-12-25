namespace NightTasker.ApiGateway.Settings;

public record RouteConfigurationSettings(
    RouteConfiguration Identity,
    RouteConfiguration UserHub);

public record RouteConfiguration(
    string Host, 
    int Port, 
    string OriginFilePath, 
    string GeneratedFilePath);

