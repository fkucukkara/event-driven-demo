# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY EventDrivenDemo.sln .

# Copy project files
COPY src/Shared/EventDrivenDemo.Shared/EventDrivenDemo.Shared.csproj src/Shared/EventDrivenDemo.Shared/
COPY src/APIs/InventoryAPI/InventoryAPI.csproj src/APIs/InventoryAPI/

# Restore dependencies
RUN dotnet restore src/APIs/InventoryAPI/InventoryAPI.csproj

# Copy source code
COPY src/Shared/EventDrivenDemo.Shared/ src/Shared/EventDrivenDemo.Shared/
COPY src/APIs/InventoryAPI/ src/APIs/InventoryAPI/

# Build the application
RUN dotnet build src/APIs/InventoryAPI/InventoryAPI.csproj -c Release -o /app/build

# Publish the application
RUN dotnet publish src/APIs/InventoryAPI/InventoryAPI.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "InventoryAPI.dll"]
