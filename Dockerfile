# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY DotNet-Weather.csproj .
RUN dotnet restore

# Copy everything else
COPY . .

# Publish the app
RUN dotnet publish DotNet-Weather.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Run the app
ENTRYPOINT ["dotnet", "DotNet-Weather.dll"]
