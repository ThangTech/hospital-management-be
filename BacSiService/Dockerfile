# =====================================================
# BacSiService - Multi-stage Dockerfile
# =====================================================

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["BacSiService/BacSiService.csproj", "BacSiService/"]
RUN dotnet restore "BacSiService/BacSiService.csproj"

# Copy source code and build
COPY ["BacSiService/", "BacSiService/"]
WORKDIR "/src/BacSiService"
RUN dotnet build "BacSiService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BacSiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# Copy published files
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "BacSiService.dll"]
