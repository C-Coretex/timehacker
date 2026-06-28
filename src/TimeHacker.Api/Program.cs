using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TimeHacker.Api.Converters.Input.Tasks.RepeatingEntities;
using TimeHacker.Api.Filters;
using TimeHacker.Api.Middleware;
using TimeHacker.Application.Api.Extensions;
using TimeHacker.Domain.Services.Extensions;
using TimeHacker.Infrastructure.Extensions;
using TimeHacker.Infrastructure.Identity;
using TimeHacker.Infrastructure.Identity.Extensions;
using TimeHacker.Migrations.Factory;
using TimeHacker.Migrations.Identity.Factory;

var builder = WebApplication.CreateBuilder(args);

#region Services

var timeHackerConnectionString = builder.Configuration.GetConnectionString("TimeHackerConnectionString") ?? throw new InvalidOperationException("Connection string 'TimeHackerConnectionString' not found.");
var timeHackerAdminConnectionString = builder.Configuration.GetConnectionString("TimeHackerAdminConnectionString") ?? throw new InvalidOperationException("Connection string 'TimeHackerAdminConnectionString' not found.");
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnectionString") ?? throw new InvalidOperationException("Connection string 'IdentityConnectionString' not found.");

RegisterServices(builder.Services, timeHackerConnectionString, identityConnectionString);

AddIdentityServices(builder.Services);

AddApplicationServices(builder.Services);

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDataProtection()
    .SetApplicationName("TimeHacker")
    .PersistKeysToFileSystem(new DirectoryInfo(
        builder.Configuration.GetValue<string>("DataProtection:KeysPath") ?? "/keys"));

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        var pd = context.ProblemDetails;
        pd.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        pd.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
        pd.Extensions.TryAdd("traceId", activity?.Id);
    };
});

// The token is exposed to the SPA via GET /api/antiforgery/token and returned back in the X-XSRF-TOKEN header.
// Required because auth cookies are SameSite=None (cross-site UI/API).
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.Name = "XSRF-TOKEN";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateAntiforgeryFilter>();
    options.Filters.Add<LogExceptionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new InputRepeatingEntityTypeConverter());
});

var uiUrl = builder.Configuration.GetValue<string>("AppSettings:uiUrl")!;
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(uiUrl)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddNpgSql(timeHackerConnectionString, name: "TimeHackerDb")
    .AddNpgSql(identityConnectionString, name: "IdentityDb");

builder.Services.AddEndpointsApiExplorer();

AddOpenTelemetry(builder.Logging, builder.Services);

#endregion

var app = builder.Build();

#region Middlewares

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseMigrationsEndPoint();

    //Apply database migrations
    TimeHackerMigrationsDbContext.ApplyMigrations(timeHackerAdminConnectionString);
    IdentityMigrationsDbContext.ApplyMigrations(identityConnectionString);
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.UseRouting();

app.UseSession();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserAccessorInitMiddleware>();

// Issues the antiforgery cookie and returns the request token for the SPA to return
// back in the X-XSRF-TOKEN header on state-changing requests.
app.MapGet("/api/antiforgery/token", (IAntiforgery antiforgery, HttpContext httpContext) =>
{
    var tokens = antiforgery.GetAndStoreTokens(httpContext);
    return Results.Ok(new { token = tokens.RequestToken });
}).RequireAuthorization();

app.MapIdentityApi<IdentityUser>();

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    StatusCodeSelector = ex => ex switch
    {
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        ArgumentException => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    }
});

app.MapControllers();

#endregion

app.Run();


#region Private static

static void AddOpenTelemetry(ILoggingBuilder logging, IServiceCollection services)
{
    logging.AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;

        options
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService("TimeHacker.Api"))
            .AddConsoleExporter();
    });

    services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService("TimeHacker.Api"))
        .WithTracing(tracing => 
            tracing
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter())
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter());
}

static void RegisterServices(IServiceCollection services, string dbConnectionString, string identityDbConnectionString)
{
    services.AddDatabaseDeveloperPageExceptionFilter();

    services.RegisterRepositories(dbConnectionString);
    services.RegisterIdentity(identityDbConnectionString);

    services.RegisterDomainServices();
    services.RegisterAppServices();
}

static void AddApplicationServices(IServiceCollection services)
{
    services.AddScoped<UserAccessor, UserAccessor>();
    services.AddScoped<UserAccessorBase>(provider => provider.GetRequiredService<UserAccessor>());
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
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Required if SameSite=None

        // Cookie lifetime
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // default
        options.SlidingExpiration = true;
        options.Events.OnSigningIn = context =>
        {
            if (context.Properties.IsPersistent)
                context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14); // remember me 14 days

            return Task.CompletedTask;
        };

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