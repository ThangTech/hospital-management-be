# =====================================================
# HoaDonService - Multi-stage Dockerfile
# =====================================================

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["HoaDonService/HoaDonService.csproj", "HoaDonService/"]
RUN dotnet restore "HoaDonService/HoaDonService.csproj"

# Copy source code and build
COPY ["HoaDonService/", "HoaDonService/"]
WORKDIR "/src/HoaDonService"
RUN dotnet build "HoaDonService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "HoaDonService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# Copy published files
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "HoaDonService.dll"]
