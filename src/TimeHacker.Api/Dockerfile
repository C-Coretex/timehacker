# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
# Create the Data Protection key directory owned by the non-root app user. A fresh named volume
# mounted at /keys inherits this ownership, so the app can persist its key ring.
RUN mkdir -p /keys && chown $APP_UID /keys
USER $APP_UID


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# The shared build config must be present before restore: Directory.Build.props supplies TargetFramework
# and Directory.Packages.props supplies every package version (central package management). MSBuild's
# implicit import is case-sensitive on Linux, so both names must match exactly.
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["TimeHacker.Api/TimeHacker.Api.csproj", "TimeHacker.Api/"]
COPY ["TimeHacker.Application.Api/TimeHacker.Application.Api.csproj", "TimeHacker.Application.Api/"]
COPY ["TimeHacker.Application.Api.Contracts/TimeHacker.Application.Api.Contracts.csproj", "TimeHacker.Application.Api.Contracts/"]
COPY ["TimeHacker.Domain/TimeHacker.Domain.csproj", "TimeHacker.Domain/"]
COPY ["TimeHacker.Domain.Services/TimeHacker.Domain.Services.csproj", "TimeHacker.Domain.Services/"]
COPY ["TimeHacker.Helpers.Domain/TimeHacker.Helpers.Domain.csproj", "TimeHacker.Helpers.Domain/"]
COPY ["TimeHacker.Helpers.DB/TimeHacker.Helpers.DB.csproj", "TimeHacker.Helpers.DB/"]
COPY ["TimeHacker.Infrastructure.Identity/TimeHacker.Infrastructure.Identity.csproj", "TimeHacker.Infrastructure.Identity/"]
COPY ["TimeHacker.Infrastructure/TimeHacker.Infrastructure.csproj", "TimeHacker.Infrastructure/"]
COPY ["TimeHacker.Migrations.Identity/TimeHacker.Migrations.Identity.csproj", "TimeHacker.Migrations.Identity/"]
COPY ["TimeHacker.Migrations/TimeHacker.Migrations.csproj", "TimeHacker.Migrations/"]
RUN dotnet restore "./TimeHacker.Api/TimeHacker.Api.csproj"
COPY . .
WORKDIR "/src/TimeHacker.Api"
RUN dotnet build "./TimeHacker.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TimeHacker.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "TimeHacker.Api.dll"]