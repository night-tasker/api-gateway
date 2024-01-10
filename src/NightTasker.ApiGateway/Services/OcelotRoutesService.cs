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
            _routeConfigurationSettings.UserHub,
            _routeConfigurationSettings.SwaggerEndPoints
        };
        
        Directory.CreateDirectory("GeneratedRoutes");

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
            var variableNames = SearchEnvironmentVariables(line);

            if (!variableNames.Any())
            {
                newFileLines.Add(line);
                continue;
            }
            
            var newLine = ReplaceEnvironmentVariablesInLine(line, variableNames);
            newFileLines.Add(newLine);
        }
        
        await File.WriteAllLinesAsync(routeConfiguration.GeneratedFilePath, newFileLines);
        return routeConfiguration.GeneratedFilePath;
    }
    
    private string ReplaceEnvironmentVariablesInLine(string line, IReadOnlyCollection<string> variableNames)
    {
        foreach (var variableName in variableNames)
        {
            var value = GetEnvironmentVariableValue(variableName);
            
            line = line.Replace($"${{{variableName}}}", value);
        }

        return line;
    }

    private string GetEnvironmentVariableValue(string variableName)
    {
        var value = _configuration[variableName!];
        if (value == null)
        {
            throw new InvalidOperationException($"No value for variable {variableName}");
        }

        return value;
    }
    
    private IReadOnlyCollection<string> SearchEnvironmentVariables(string line)
    {
        var envVariables = new List<string>();
        while (SearchEnvironmentVariable(line, out string? variableName, out string restString))
        {
            line = restString;
            envVariables.Add(variableName!);
        }

        return envVariables;
    }
    
    private bool SearchEnvironmentVariable(string line, out string? variableName, out string restString)
    {
        var indexOfDollar = line.IndexOf('$');

        if (indexOfDollar == -1)
        {
            variableName = null;
            restString = line;
            return false;
        }
        
        var substring = line.Substring(indexOfDollar + 1);
        if (substring[0] != '{')
        {
            throw new InvalidOperationException("After $ there must be a {");
        }
            
        var indexOfClosingBrace = substring.IndexOf('}');
        if (indexOfClosingBrace == -1)
        {
            throw new InvalidOperationException("After { there must be a }");
        }
        restString = line.Remove(indexOfDollar, indexOfClosingBrace - indexOfDollar + 1); 
        variableName = substring.Substring(1, indexOfClosingBrace - 1);
        return true;
    }
}