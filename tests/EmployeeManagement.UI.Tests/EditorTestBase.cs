using EmployeeManagement.Data;
using EmployeeManagement.Data.Entities;
using EmployeeManagement.Data.Repositories;
using EmployeeManagement.Shared.Models;
using EmployeeManagement.Shared.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.UI.Tests;

public abstract class EditorTestBase
{
    private SqliteConnection _connection = null!;
    private ServiceProvider _provider = null!;

    protected EmployeeEditor ViewModel { get; private set; } = null!;
    protected int DepartmentId { get; private set; }
    protected int PositionId { get; private set; }

    [TestInitialize]
    public void SetUp()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var services = new ServiceCollection();
        services.AddDbContext<EmployeeDbContext>(options => options.UseSqlite(_connection));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        _provider = services.BuildServiceProvider();

        using (var scope = _provider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
            context.Database.EnsureCreated();

            var department = new Department { Name = "IT" };
            var position = new Position { Title = "Developer" };
            context.Departments.Add(department);
            context.Positions.Add(position);
            context.SaveChanges();

            DepartmentId = department.Id;
            PositionId = position.Id;
        }

        ViewModel = new EmployeeEditor(_provider.GetRequiredService<IServiceScopeFactory>());
    }

    [TestCleanup]
    public void TearDown()
    {
        _provider.Dispose();
        _connection.Dispose();
    }

    protected EmployeeRow CreateRow(string lastName = "Sample", string? email = null) => new()
    {
        FirstName = "Max",
        LastName = lastName,
        Email = email ?? $"{lastName.ToLowerInvariant()}@example.com",
        HireDate = new DateOnly(2024, 1, 15),
        Salary = 50000m,
        DepartmentId = DepartmentId,
        PositionId = PositionId
    };

    protected void SeedEmployee(string lastName, bool isDeleted = false)
    {
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
        context.Employees.Add(new Employee
        {
            FirstName = "Max",
            LastName = lastName,
            Email = $"{lastName.ToLowerInvariant()}@example.com",
            HireDate = new DateOnly(2024, 1, 15),
            Salary = 50000m,
            DepartmentId = DepartmentId,
            PositionId = PositionId,
            IsDeleted = isDeleted
        });
        context.SaveChanges();
    }

    protected List<Employee> ReadAllEmployees()
    {
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
        return context.Employees.IgnoreQueryFilters().AsNoTracking().ToList();
    }
}
