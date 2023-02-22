# syntax = docker/dockerfile:1.2
ARG NET_IMAGE=6.0-bullseye-slim

FROM mcr.microsoft.com/dotnet/aspnet:${NET_IMAGE} AS base
WORKDIR "/app"

FROM mcr.microsoft.com/dotnet/sdk:${NET_IMAGE} AS build
WORKDIR "/src"

# Copy local **.csproj files to image and restore
COPY ["Zue.ScheduledWorker/Zue.ScheduledWorker.csproj", "Zue.ScheduledWorker/"]
COPY ["Zue.PeriodicScheduler/Zue.PeriodicScheduler.csproj", "Zue.PeriodicScheduler/"]
RUN dotnet restore "Zue.ScheduledWorker/Zue.ScheduledWorker.csproj"

# Copy everything else not in .dockerignore and build
COPY . .
RUN dotnet build "Zue.ScheduledWorker/Zue.ScheduledWorker.csproj" --configuration Release --no-restore

FROM build AS publish
RUN dotnet publish "Zue.ScheduledWorker/Zue.ScheduledWorker.csproj" -c Release --output "published/" --no-build --no-self-contained

FROM base AS final
COPY --from=publish "src/published" .
ENTRYPOINT ["dotnet", "Zue.ScheduledWorker.dll"]
