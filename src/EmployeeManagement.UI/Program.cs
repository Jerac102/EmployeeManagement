using EmployeeManagement.Data;
using EmployeeManagement.UI.Forms;
using EmployeeManagement.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            var services = new ServiceCollection();
            services.AddDataAccess(connectionString);
            services.AddTransient<IEmployeeEditor, EmployeeEditor>();
            services.AddTransient<EmployeeListForm>();

            using var provider = services.BuildServiceProvider();
            Application.Run(provider.GetRequiredService<EmployeeListForm>());
        }
    }
}