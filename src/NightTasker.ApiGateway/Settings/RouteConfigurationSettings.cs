namespace NightTasker.ApiGateway.Settings;

public record RouteConfigurationSettings(
    RouteConfiguration Identity,
    RouteConfiguration UserHub,
    RouteConfiguration SwaggerEndPoints);

public record RouteConfiguration(
    string Host, 
    int Port, 
    string OriginFilePath, 
    string GeneratedFilePath);