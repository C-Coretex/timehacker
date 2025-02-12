using Microsoft.AspNetCore.Identity;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TimeHacker.Application.Helpers;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Extensions;
using TimeHacker.Infrastructure.Extensions;
using TimeHacker.Infrastructure.Identity;
using TimeHacker.Infrastructure.Identity.Extensions;
using TimeHacker.Migrations.Factory;
using TimeHacker.Migrations.Identity.Factory;

var builder = WebApplication.CreateBuilder(args);

#region Services


var timeHackerConnectionString = builder.Configuration.GetConnectionString("TimeHackerConnectionString") ?? throw new InvalidOperationException("Connection string 'TimeHackerConnectionString' not found.");
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnectionString") ?? throw new InvalidOperationException("Connection string 'IdentityConnectionString' not found.");

AddDbServices(builder.Services, timeHackerConnectionString, identityConnectionString);

AddIdentityServices(builder.Services);

AddApplicationServices(builder.Services);

builder.Services.AddControllersWithViews();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddNpgSql(timeHackerConnectionString, name: "TimeHackerDb")
    .AddNpgSql(identityConnectionString, name: "IdentityDb");

AddOpenTelemetry(builder.Logging, builder.Services);

#endregion

var app = builder.Build();

#region Middlewares

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseMigrationsEndPoint();

    //Apply database migrations
    TimeHackerMigrationsDbContext.ApplyMigrations(timeHackerConnectionString);
    IdentityMigrationsDbContext.ApplyMigrations(identityConnectionString);
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapHealthChecks("/health");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapIdentityApi<IdentityUser>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#endregion

app.Run();


#region Private static

static void AddOpenTelemetry(ILoggingBuilder logging, IServiceCollection services)
{
    logging.AddOpenTelemetry(options =>
    {
        options
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService("TimeHacker.Application"))
            .AddConsoleExporter();
    });
    services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService("TimeHacker.Application"))
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter())
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter());
    logging.AddOpenTelemetry(options =>
    {
        options
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService("TimeHacker.Application"))
            .AddConsoleExporter();
    });
    services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService("TimeHacker.Application"))
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter())
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter());
}

static void AddDbServices(IServiceCollection services, string dbConnectionString, string identityDbConnectionString)
{
    services.AddDatabaseDeveloperPageExceptionFilter();

    services.RegisterRepositories(dbConnectionString);
    services.RegisterIdentity(identityDbConnectionString);

    services.RegisterServices();
}

static void AddApplicationServices(IServiceCollection services)
{
    services.AddScoped<UserAccessorBase, UserAccessor>();
}

static void AddIdentityServices(IServiceCollection services)
{
    services.AddAuthorization();
    services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);

    services.AddIdentityCore<IdentityUser>(o =>
        {
            o.Password.RequireDigit = true;
            o.Password.RequiredLength = 6;
            o.Password.RequireLowercase = true;
            o.Password.RequireUppercase = true;
            o.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<TimeHackerIdentityDbContext>()
        .AddApiEndpoints();

    services.ConfigureApplicationCookie(options =>
    {
        // Prevent automatic redirects
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
}

#endregion