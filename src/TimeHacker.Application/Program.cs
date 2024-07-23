using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Application.Helpers;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Infrastructure;
using TimeHacker.Persistence.Extensions;
using TimeHacker.Persistence.IdentityData;
using TimeHacker.Domain.Extensions;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

#region DB
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnectionString") ?? throw new InvalidOperationException("Connection string 'IdentityConnectionString' not found.");
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(identityConnectionString));

var timeHackerConnectionString = builder.Configuration.GetConnectionString("TimeHackerConnectionString") ?? throw new InvalidOperationException("Connection string 'TimeHackerConnectionString' not found.");

builder.Services.RegisterRepositories(timeHackerConnectionString);
builder.Services.RegisterServices();
#endregion

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(o =>
{
    o.Password.RequireDigit = true;
    o.Password.RequiredLength = 6;
    o.Password.RequireLowercase = true;
    o.Password.RequireUppercase = true;
    o.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<IdentityDbContext>();

AddApplicationServices(builder.Services);

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.WebHost.UseStaticWebAssets();

var mapper = GetMapperConfiguration().CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

static IServiceCollection AddApplicationServices(IServiceCollection services)
{
    services.AddScoped<IUserAccessor, UserAccessor>();

    return services;
}

static MapperConfiguration GetMapperConfiguration()
{
    var types = AppDomain.CurrentDomain
                        .GetAssemblies()
                        .Where(x => x.FullName!.StartsWith("TimeHacker."))
                        .SelectMany(s => s.GetTypes())
                        .Where(p => typeof(Profile).IsAssignableFrom(p));

    return new MapperConfiguration(cfg =>
    {
        foreach (var type in types)
            cfg.AddProfile(type);
    });
}