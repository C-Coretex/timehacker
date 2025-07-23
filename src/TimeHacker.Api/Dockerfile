# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TimeHacker.Api/TimeHacker.Api.csproj", "TimeHacker.Api/"]
COPY ["TimeHacker.Domain/TimeHacker.Domain.csproj", "TimeHacker.Domain/"]
COPY ["TimeHacker.Domain.Services/TimeHacker.Domain.Services.csproj", "TimeHacker.Domain.Services/"]
COPY ["TimeHacker.Helpers.Domain/TimeHacker.Helpers.Domain.csproj", "TimeHacker.Helpers.Domain/"]
COPY ["TimeHacker.Helpers.Db/TimeHacker.Helpers.Db.csproj", "TimeHacker.Helpers.DB/"]
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