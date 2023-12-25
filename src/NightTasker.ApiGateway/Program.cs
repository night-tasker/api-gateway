using NightTasker.ApiGateway.Configuration;
using NightTasker.ApiGateway.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfigurationFiles().Wait();
builder.Services.AddDefaultCorsPolicy();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOcelot();

builder.Services.AddSettings(builder.Configuration);

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
