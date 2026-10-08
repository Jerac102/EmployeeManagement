using EmployeeManagement.Data;
using EmployeeManagement.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmployeeManagement.Api.Tests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public int DepartmentId { get; private set; }
    public int PositionId { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:EmployeeManagement"] = "Server=unused;Database=unused"
            }));

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<EmployeeDbContext>>();
            services.RemoveAll<EmployeeDbContext>();
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<EmployeeDbContext>));
            services.AddDbContext<EmployeeDbContext>(options => options.UseSqlite(_connection));
        });
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var department = new Department { Name = "IT" };
        var position = new Position { Title = "Developer" };
        context.Departments.Add(department);
        context.Positions.Add(position);
        context.SaveChanges();

        DepartmentId = department.Id;
        PositionId = position.Id;
    }

    public List<Employee> ReadAllEmployees()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
        return context.Employees.IgnoreQueryFilters().AsNoTracking().ToList();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
