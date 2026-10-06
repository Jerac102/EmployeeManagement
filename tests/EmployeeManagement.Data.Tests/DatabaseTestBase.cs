using System.Text.RegularExpressions;
using EmployeeManagement.Data.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data.Tests;

public abstract class DatabaseTestBase
{
    private const string ServerConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=true;";

    private const string ScriptDatabaseName = "EmployeeManagement";

    private string _databaseName = null!;

    protected EmployeeDbContext Context { get; private set; } = null!;

    [TestInitialize]
    public void SetUp()
    {
        _databaseName = $"EmployeeManagement_Test_{Guid.NewGuid():N}";
        ExecuteScript(_databaseName);

        var options = new DbContextOptionsBuilder<EmployeeDbContext>()
            .UseSqlServer($"{ServerConnectionString}Database={_databaseName};")
            .Options;

        Context = new EmployeeDbContext(options);
    }

    [TestCleanup]
    public void TearDown()
    {
        Context.Dispose();
        SqlConnection.ClearAllPools();

        using var connection = new SqlConnection(ServerConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            $"ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{_databaseName}];";
        command.ExecuteNonQuery();
    }

    private static void ExecuteScript(string databaseName)
    {
        var scriptPath = Path.Combine(AppContext.BaseDirectory, "Sql", "CreateDatabase.sql");
        var script = File.ReadAllText(scriptPath).Replace(ScriptDatabaseName, databaseName);
        var batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        using var connection = new SqlConnection(ServerConnectionString);
        connection.Open();

        foreach (var batch in batches.Where(b => !string.IsNullOrWhiteSpace(b)))
        {
            using var command = connection.CreateCommand();
            command.CommandText = batch;
            command.ExecuteNonQuery();
        }
    }

    protected (Department Department, Position Position) SeedLookups()
    {
        var department = new Department { Name = "IT" };
        var position = new Position { Title = "Developer" };
        Context.Departments.Add(department);
        Context.Positions.Add(position);
        Context.SaveChanges();
        return (department, position);
    }

    protected static Employee CreateEmployee(Department department, Position position, string lastName = "Sample")
        => new()
        {
            FirstName = "Max",
            LastName = lastName,
            Email = $"{lastName.ToLowerInvariant()}@example.com",
            HireDate = new DateTime(2024, 1, 15),
            Salary = 50000m,
            DepartmentId = department.Id,
            PositionId = position.Id
        };
}
