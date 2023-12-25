using Microsoft.Extensions.Options;
using NightTasker.ApiGateway.Contracts;
using NightTasker.ApiGateway.Settings;

namespace NightTasker.ApiGateway.Services;

public class OcelotRoutesService(
    IConfiguration configuration,
    RouteConfigurationSettings routeConfigurationSettings) : IOcelotRoutesService
{
    private readonly IConfiguration _configuration =
        configuration ?? throw new ArgumentNullException(nameof(configuration));

    private readonly RouteConfigurationSettings _routeConfigurationSettings = 
        routeConfigurationSettings ?? throw new ArgumentNullException(nameof(routeConfigurationSettings));
    public Task<string[]> GenerateOcelotFiles()
    {
        var routeConfigurations = new[]
        {
            _routeConfigurationSettings.Identity,
            _routeConfigurationSettings.UserHub
        };

        Task<string>[] tasks = routeConfigurations
            .Select(ReplaceEnvironmentVariablesInFile)
            .ToArray();

        return Task.WhenAll(tasks);
    }
    private async Task<string> ReplaceEnvironmentVariablesInFile(
        RouteConfiguration routeConfiguration)
    {
        var newFileLines = new List<string>();
        
        await foreach (var line in File.ReadLinesAsync(routeConfiguration.OriginFilePath))
        {
            var variableName = SearchEnvironmentVariable(line);

            if (variableName is null)
            {
                newFileLines.Add(line);
                continue;
            }

            var value = GetEnvironmentVariableValue(variableName!);
            
            var newLine = line.Replace($"${{{variableName}}}", value);
            newFileLines.Add(newLine);
        }
        
        await File.WriteAllLinesAsync(routeConfiguration.GeneratedFilePath, newFileLines);
        return routeConfiguration.GeneratedFilePath;
    }

    private string GetEnvironmentVariableValue(string variableName)
    {
        var value = configuration[variableName!];
        if (value == null)
        {
            throw new InvalidOperationException($"No value for variable {variableName}");
        }

        return value;
    }
    
    private string? SearchEnvironmentVariable(string line)
    {
        var index = line.IndexOf('$');
        if (index == -1)
        {
            return null;
        }
            
        var substring = line.Substring(index + 1);
        if (substring[0] != '{')
        {
            throw new InvalidOperationException("After $ there must be a {");
        }
            
        var indexOfClosingBrace = substring.IndexOf('}');
        if (indexOfClosingBrace == -1)
        {
            throw new InvalidOperationException("After { there must be a }");
        }
        
        var variableName = substring.Substring(1, indexOfClosingBrace - 1);
        return variableName;
    }
    
    private string GetOriginFilePath()
    {
        var filePath = _configuration["RoutesConfiguration:FilePath:Origin"];
        if (filePath is null)
        {
            throw new InvalidOperationException("No value for RoutesConfiguration:FilePath:Origin");
        }
        
        return filePath;
    }

    private string GetGeneratedFilePath()
    {
        var filePath = _configuration["RoutesConfiguration:FilePath:Generated"];
        if (filePath is null)
        {
            throw new InvalidOperationException("No value for RoutesConfiguration:FilePath:Generated");
        }
        
        return filePath;
    }
}