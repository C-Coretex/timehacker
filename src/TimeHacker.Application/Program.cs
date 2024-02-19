using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Application.Helpers;
using TimeHacker.Domain.Abstractions.Interfaces.Helpers;
using TimeHacker.Domain.BusinessLogic.Services;
using TimeHacker.Persistence.Context;
using TimeHacker.Persistence.Extensions;
using TimeHacker.Persistence.IdentityData;

var builder = WebApplication.CreateBuilder(args);

#region DB
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnectionString") ?? throw new InvalidOperationException("Connection string 'IdentityConnectionString' not found.");
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(identityConnectionString));

var timeHackerConnectionString = builder.Configuration.GetConnectionString("TimeHackerConnectionString") ?? throw new InvalidOperationException("Connection string 'TimeHackerConnectionString' not found.");
builder.Services.AddDbContext<TimeHackerDBContext>(options =>
    options.UseSqlServer(timeHackerConnectionString));

builder.Services.AddTimeHackerPersistenceServices();
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

AddBusinessLogicServices(builder.Services);

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.WebHost.UseStaticWebAssets();

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

static IServiceCollection AddBusinessLogicServices(IServiceCollection services)
{
    services.AddScoped<TasksService>();
    services.AddScoped<IUserAccessor, UserAccessor>();
    return services;
}