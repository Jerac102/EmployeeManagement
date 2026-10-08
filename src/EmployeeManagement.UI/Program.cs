using EmployeeManagement.Data;
using EmployeeManagement.UI.Forms;
using EmployeeManagement.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EmployeeManagement.UI
{
    internal static class Program
    {
        private const string ConnectionStringVariable = "EMPLOYEEMANAGEMENT_CONNECTION";
        private const string ConnectionStringName = "EmployeeManagement";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = Environment.GetEnvironmentVariable(ConnectionStringVariable)
                ?? configuration.GetConnectionString(ConnectionStringName)
                ?? throw new InvalidOperationException(
                    $"Connection string '{ConnectionStringName}' is not configured in appsettings.json.");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    Path.Combine(AppContext.BaseDirectory, "logs", "ui-.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Application.ThreadException += (_, e) => Log.Error(e.Exception, "Unhandled UI thread exception");
            AppDomain.CurrentDomain.UnhandledException += (_, e) => Log.Fatal(e.ExceptionObject as Exception, "Unhandled exception");

            var services = new ServiceCollection();
            services.AddLogging(logging => logging.AddSerilog(dispose: true));
            services.AddDataAccess(connectionString);
            services.AddTransient<IEmployeeEditor, EmployeeEditor>();
            services.AddTransient<EmployeeListForm>();

            using var provider = services.BuildServiceProvider();
            Log.Information("Employee Management UI starting");
            try
            {
                Application.Run(provider.GetRequiredService<EmployeeListForm>());
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "UI terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.Information("Employee Management UI closed");
                Log.CloseAndFlush();
            }
        }
    }
}