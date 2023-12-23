using NightTasker.ApiGateway.Configuration;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfigurationFiles();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOcelot();

var app = builder.Build();

app.UseOcelot();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
