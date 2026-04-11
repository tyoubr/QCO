using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using QCO.Data;
using QCO.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Retrieve the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register QCOContext with the DI container (for your application data)
builder.Services.AddDbContext<QCOContext>(options =>
    options.UseSqlServer(connectionString));

// Register ApplicationDbContext for Identity (for authentication/authorization)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();
// Add exception filter (for development environment)
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register controllers with views
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<OracleContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));
builder.Services.AddDevExpressControls();
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // Enables migrations error page
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Handles errors with a friendly page
    app.UseHsts(); // Enforces strict HTTPS in production
}

app.UseDevExpressControls();
System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
app.UseHttpsRedirection(); // Redirects HTTP to HTTPS
app.UseStaticFiles(); // Serves static files like CSS, JS, images

app.UseRouting(); // Enables routing middleware

app.UseAuthentication(); // Adds authentication middleware
app.UseAuthorization(); // Adds authorization middleware

// Configure route mappings
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages (needed for Identity)
app.MapRazorPages();

// Run the application
app.Run();
