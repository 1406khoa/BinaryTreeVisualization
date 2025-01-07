# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BinaryTreeVisualization.csproj", "./"]
RUN dotnet restore "./BinaryTreeVisualization.csproj"
COPY . /src/
WORKDIR "/src/"
RUN dotnet build "./BinaryTreeVisualization.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BinaryTreeVisualization.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BinaryTreeVisualization.dll"]
