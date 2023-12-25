namespace NightTasker.ApiGateway.Contracts;

public interface IOcelotRoutesService
{ 
    Task<string[]> GenerateOcelotFiles();
}