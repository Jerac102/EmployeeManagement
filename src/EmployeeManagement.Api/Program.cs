using EmployeeManagement.Data;
using Serilog;

const string ConnectionStringVariable = "EMPLOYEEMANAGEMENT_CONNECTION";
const string ConnectionStringName = "EmployeeManagement";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.File(
        Path.Combine(AppContext.BaseDirectory, "logs", "api-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"));

var connectionString = Environment.GetEnvironmentVariable(ConnectionStringVariable)
    ?? builder.Configuration.GetConnectionString(ConnectionStringName)
    ?? throw new InvalidOperationException(
        $"Connection string '{ConnectionStringName}' is not configured in appsettings.json.");

// Add services to the container.
builder.Services.AddDataAccess(connectionString);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
public partial class Program;
