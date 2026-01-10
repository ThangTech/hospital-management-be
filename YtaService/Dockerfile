# =====================================================
# YtaService - Multi-stage Dockerfile
# =====================================================

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["YtaService/YtaService.csproj", "YtaService/"]
RUN dotnet restore "YtaService/YtaService.csproj"

# Copy source code and build
COPY ["YtaService/", "YtaService/"]
WORKDIR "/src/YtaService"
RUN dotnet build "YtaService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "YtaService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# Copy published files
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "YtaService.dll"]
