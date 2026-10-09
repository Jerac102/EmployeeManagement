using EmployeeManagement.Data;
using EmployeeManagement.Shared.Services;
using EmployeeManagement.Web.Components;
using Serilog;

const string ConnectionStringVariable = "EMPLOYEEMANAGEMENT_CONNECTION";
const string ConnectionStringName = "EmployeeManagement";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(AppContext.BaseDirectory, "logs", "web-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"));

var connectionString = Environment.GetEnvironmentVariable(ConnectionStringVariable)
    ?? builder.Configuration.GetConnectionString(ConnectionStringName)
    ?? throw new InvalidOperationException(
        $"Connection string '{ConnectionStringName}' is not configured in appsettings.json.");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDataAccess(connectionString);
builder.Services.AddScoped<IEmployeeEditor, EmployeeEditor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSerilogRequestLogging();
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
