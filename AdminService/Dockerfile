# =====================================================
# AdminService - Multi-stage Dockerfile
# =====================================================

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["AdminService/AdminService.csproj", "AdminService/"]
RUN dotnet restore "AdminService/AdminService.csproj"

# Copy source code and build
COPY ["AdminService/", "AdminService/"]
WORKDIR "/src/AdminService"
RUN dotnet build "AdminService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AdminService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# Copy published files
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AdminService.dll"]
