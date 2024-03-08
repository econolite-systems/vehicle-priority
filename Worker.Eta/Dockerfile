FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["vehicle-priority/Worker.Eta/Worker.Eta.csproj", "Worker.Eta/"]
RUN dotnet restore "vehicle-priority/Worker.Eta/Worker.Eta.csproj"
COPY . .
WORKDIR "/src/Worker.Eta"
RUN dotnet build "Worker.Eta.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Worker.Eta.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Worker.Eta.dll"]
