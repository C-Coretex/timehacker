using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TimeHacker.Api.Converters;
using TimeHacker.Api.Converters.Input.Tasks.RepeatingEntities;
using TimeHacker.Api.Filters;
using TimeHacker.Api.Middleware;
using TimeHacker.Api.Seeding;
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

if (new NpgsqlConnectionStringBuilder(timeHackerConnectionString).NoResetOnClose)
{
    throw new InvalidOperationException(
        "No Reset On Close=true is incompatible with session-scoped RLS state in UserSessionInterceptor." +
        "Either set it back to false, or migrate the interceptor to transaction-scoped set_config(..., true) first.");
}

RegisterServices(builder.Services, timeHackerConnectionString, identityConnectionString);

AddIdentityServices(builder.Services);

AddApplicationServices(builder.Services);

builder.Services.AddSingleton(TimeProvider.System);

// Persist the Data Protection key ring so encrypted cookies (Identity, Session, Antiforgery)
// survive API/container restarts. Without this the container generates an in-memory key ring on
// every start and can no longer decrypt previously issued cookies.
builder.Services.AddDataProtection()
    .SetApplicationName("TimeHacker")
    .PersistKeysToFileSystem(new DirectoryInfo(
        builder.Configuration.GetValue<string>("DataProtection:KeysPath") ?? "/keys"));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    // SameSite=None (with Secure) lets the cross-origin SPA send the session cookie to the API over HTTPS.
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
});

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
}).AddJsonOptions(options => //for MVC controllers functionality (input binding [FromBody], Non-TypedResults return])
{
    options.JsonSerializerOptions.Converters.Add(new InputRepeatingEntityTypeConverter());
    AddSharedJsonConverters(options.JsonSerializerOptions);
});

// Controllers return results via TypedResults (HttpResults), which serialize with the Http.Json options
// rather than the MVC AddJsonOptions above — so response bodies (e.g. Color) need the converter here too
// (for Minimal APIs, TypedResults, HttpContext).
builder.Services.ConfigureHttpJsonOptions(options =>
{
    AddSharedJsonConverters(options.SerializerOptions);
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
    // Reuse the app's named NpgsqlDataSources instead of raw connection strings, so health-check DB metrics
    // carry the clean pool names ("TimeHacker"/"TimeHackerIdentity") rather than the connection string, and
    // no redundant unnamed data source/pool is created.
    .AddNpgSql(sp => sp.GetRequiredKeyedService<NpgsqlDataSource>("TimeHacker"), name: "TimeHackerDb")
    .AddNpgSql(sp => sp.GetRequiredKeyedService<NpgsqlDataSource>("TimeHackerIdentity"), name: "IdentityDb");

builder.Services.AddEndpointsApiExplorer();

AddOpenTelemetry(builder.Logging, builder.Configuration, builder.Services, builder.Environment);

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

    // Seed the sample account with sample data (idempotent) so a fresh DB is usable immediately.
    await DevelopmentDataSeeder.SeedAsync(app.Services, timeHackerAdminConnectionString);
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

// Must come after authentication/authorization: it reads the authenticated principal's claims to resolve
// (and lazily create) the domain User before any controller runs.
app.UseMiddleware<UserAccessorInitMiddleware>();

// Issues the antiforgery cookie and returns the request token for the SPA to return
// back in the X-XSRF-TOKEN header on state-changing requests.
app.MapGet("/api/antiforgery/token", (IAntiforgery antiforgery, HttpContext httpContext) =>
{
    var tokens = antiforgery.GetAndStoreTokens(httpContext);
    return Results.Ok(new { token = tokens.RequestToken });
}).RequireAuthorization();

app.MapIdentityApi<IdentityUser>();

// MapIdentityApi ships register/login/refresh/etc, but no logout. It signs out the
// Identity cookie and clears the session — the session caches the resolved domain UserId, so clearing it
// prevents a later login on the same browser session from inheriting the previous user's id.
app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager, HttpContext httpContext) =>
{
    await signInManager.SignOutAsync();
    httpContext.Session.Clear();
    return Results.Ok();
}).RequireAuthorization();

// Status mapping for non-MVC (minimal-API) endpoints such as MapIdentityApi, which don't pass through
// LogExceptionFilter. The richer domain-exception mapping for controllers lives in LogExceptionFilter.
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

await app.RunAsync();

#region Private static

static void AddOpenTelemetry(ILoggingBuilder logging, ConfigurationManager configuration, IServiceCollection services, IHostEnvironment environment)
{
    const string ServiceName = "TimeHacker.Api";

    // Export over OTLP when an endpoint is configured (Docker/production); otherwise fall back to the
    // console exporter so unit/integration tests and a bare `dotnet run` don't try to reach a collector.
    // The plain OtlpExporter reads OTEL_EXPORTER_OTLP_ENDPOINT / OTEL_EXPORTER_OTLP_PROTOCOL itself.
    var otlpEndpoint = configuration["OTEL:ExporterOtlpEndpoint"] 
        ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
    var useOtlp = !string.IsNullOrWhiteSpace(otlpEndpoint);

    var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

    // Identify the service across all three signals so Grafana can filter by version, environment, and
    // instance. Applied to logs (SetResourceBuilder) and traces/metrics (ConfigureResource) alike.
    void ConfigureResource(ResourceBuilder resource) => resource
        .AddService(ServiceName, serviceVersion: serviceVersion, serviceInstanceId: Environment.MachineName)
        .AddAttributes([new("deployment.environment", environment.EnvironmentName)]);

    logging.AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;

        var resource = ResourceBuilder.CreateDefault();
        ConfigureResource(resource);
        options.SetResourceBuilder(resource);

        if (useOtlp)
            options.AddOtlpExporter();
        else
            options.AddConsoleExporter();
    });

    // The OTLP log exporter above already ships every record (info -> error) to the backend, so
    // exceptions are visible in Grafana. Additionally mirror error-level records (which is where
    // unhandled exceptions are logged, see LogExceptionFilter) to the console so they also stay
    // visible in the terminal, without console-spamming routine info/warning logs.
    if (useOtlp)
    {
        logging.AddConsole();
        logging.AddFilter<Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider>(category: null, LogLevel.Error);
    }

    services.AddOpenTelemetry()
        .ConfigureResource(ConfigureResource)
        .WithTracing(tracing =>
        {
            tracing
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddSource("Npgsql") // per-command DB spans (query duration), correlated into the request trace
                .AddSource(TimeHackerTelemetry.ActivitySourceName); // business spans (e.g. timeline.generate)

            if (useOtlp)
                tracing.AddOtlpExporter();
            else
                tracing.AddConsoleExporter();
        })
        .WithMetrics(metrics =>
        {
            metrics
                // Attach the active trace id to metric samples recorded inside a sampled span, so Grafana
                // can jump metric -> trace via exemplars (needs Prometheus --enable-feature=exemplar-storage
                // and the Prometheus datasource's exemplarTraceIdDestinations).
                .SetExemplarFilter(ExemplarFilterType.TraceBased)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation() // outbound HTTP request metrics
                .AddRuntimeInstrumentation() // GC, thread pool, memory
                .AddMeter("Npgsql") // DB command duration + connection-pool metrics
                .AddMeter("Microsoft.EntityFrameworkCore") // EF query/compilation counts on top of Npgsql
                .AddMeter(TimeHackerTelemetry.MeterName); // business + usage metrics (see TimeHackerTelemetry / ActiveUserTracker)

            ActiveUserTracker.EnsureInitialized();

            if (useOtlp)
                metrics.AddOtlpExporter();
            else
                metrics.AddConsoleExporter();
        });
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

        // Cookie lifetime: by default a 60-minute window that slides forward on each request. For a
        // "remember me" (persistent) sign-in, OnSigningIn instead pins a fixed 14-day absolute expiry.
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Events.OnSigningIn = context =>
        {
            if (context.Properties.IsPersistent)
                context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14); // remember me 14 days

            return Task.CompletedTask;
        };

        // This is an API for a SPA, so suppress Identity's default browser redirects to login/access-denied
        // pages and return bare 401/403 status codes instead — the SPA owns the auth UI.
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

static void AddSharedJsonConverters(JsonSerializerOptions o)
{
    o.Converters.Add(new ColorJsonConverter());
    o.Converters.Add(new DateTimeUtcJsonConverter());
}

#endregion