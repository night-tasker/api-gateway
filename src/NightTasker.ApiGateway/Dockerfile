FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NightTasker.ApiGateway/NightTasker.ApiGateway.csproj", "NightTasker.ApiGateway/"]
RUN dotnet restore "NightTasker.ApiGateway/NightTasker.ApiGateway.csproj"
COPY . .
WORKDIR "/src/NightTasker.ApiGateway"
RUN dotnet build "NightTasker.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NightTasker.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NightTasker.ApiGateway.dll"]
