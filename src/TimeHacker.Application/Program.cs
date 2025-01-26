using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
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

#region DB

var timeHackerConnectionString = builder.Configuration.GetConnectionString("TimeHackerConnectionString") ?? throw new InvalidOperationException("Connection string 'TimeHackerConnectionString' not found.");
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnectionString") ?? throw new InvalidOperationException("Connection string 'IdentityConnectionString' not found.");

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.RegisterRepositories(timeHackerConnectionString);
builder.Services.RegisterIdentity(identityConnectionString);

builder.Services.RegisterServices();

#endregion

builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);

builder.Services.AddIdentityCore<IdentityUser>(o =>
    {
        o.Password.RequireDigit = true;
        o.Password.RequiredLength = 6;
        o.Password.RequireLowercase = true;
        o.Password.RequireUppercase = true;
        o.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<TimeHackerIdentityDbContext>()
    .AddApiEndpoints();

builder.Services.ConfigureApplicationCookie(options =>
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

AddApplicationServices(builder.Services);

builder.Services.AddControllersWithViews();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

#endregion

var app = builder.Build();

#region Middleware

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapScalarApiReference(o =>
    {
        o.WithTitle("TimeHacker API");
    });

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

static void AddApplicationServices(IServiceCollection services)
{
    services.AddScoped<IUserAccessor, UserAccessor>();
}