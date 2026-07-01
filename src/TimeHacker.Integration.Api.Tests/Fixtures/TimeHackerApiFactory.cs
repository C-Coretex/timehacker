

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

internal sealed class TimeHackerApiFactory : WebApplicationFactory<Program>
{
    // Program.cs reads the connection strings from builder. Configuration DURING top-level statement
    // execution (before builder.Build()). WebApplicationFactory.ConfigureWebHost -> ConfigureAppConfiguration
    // is only applied AT/AFTER Build(), so an in-memory config source there is too late — Program would read
    // appsettings.json's empty strings. Environment variables are read immediately by WebApplication.CreateBuilder
    // (via AddEnvironmentVariables(), no prefix, "__" => ":"), so setting them before the host builds is the
    // reliable way to inject the container connection strings. The host builds lazily on the first CreateClient(),
    // and this factory is constructed before any client, so the ctor is early enough.
    public TimeHackerApiFactory(string dbAppConnection, string dbAdminConnection, string dbIdentityConnection, string keysPath)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__TimeHackerConnectionString", dbAppConnection);
        Environment.SetEnvironmentVariable("ConnectionStrings__TimeHackerAdminConnectionString", dbAdminConnection);
        Environment.SetEnvironmentVariable("ConnectionStrings__IdentityConnectionString", dbIdentityConnection);
        Environment.SetEnvironmentVariable("AppSettings__uiUrl", "https://localhost:5173");
        Environment.SetEnvironmentVariable("DataProtection__KeysPath", keysPath);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        base.ConfigureWebHost(builder);
    }
}
