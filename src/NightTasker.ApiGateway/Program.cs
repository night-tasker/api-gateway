using NightTasker.ApiGateway.Configuration;
using NightTasker.ApiGateway.Constants;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfigurationFiles();
builder.Services.AddDefaultCorsPolicy();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOcelot();

var app = builder.Build();

app.UseCors(CorsConstants.DefaultCorsPolicyName);

app.ConfigureOcelot();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
