using EmployeeManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data.Tests;

[TestClass]
public class UniquenessTests : DatabaseTestBase
{
    [TestMethod]
    public void SaveChanges_DuplicateDepartmentName_Throws()
    {
        Context.Departments.Add(new Department { Name = "IT" });
        Context.SaveChanges();

        Context.Departments.Add(new Department { Name = "IT" });

        Assert.Throws<DbUpdateException>(() => Context.SaveChanges());
    }

    [TestMethod]
    public void SaveChanges_DuplicatePositionTitle_Throws()
    {
        Context.Positions.Add(new Position { Title = "Developer" });
        Context.SaveChanges();

        Context.Positions.Add(new Position { Title = "Developer" });

        Assert.Throws<DbUpdateException>(() => Context.SaveChanges());
    }

    [TestMethod]
    public void SaveChanges_DuplicateEmployeeEmail_Throws()
    {
        var (department, position) = SeedLookups();
        Context.Employees.Add(CreateEmployee(department, position, "One"));
        Context.SaveChanges();

        var duplicate = CreateEmployee(department, position, "Two");
        duplicate.Email = "one@example.com";
        Context.Employees.Add(duplicate);

        Assert.Throws<DbUpdateException>(() => Context.SaveChanges());
    }

    [TestMethod]
    public void SaveChanges_MultipleEmployeesWithoutEmail_Succeeds()
    {
        var (department, position) = SeedLookups();
        var first = CreateEmployee(department, position, "One");
        var second = CreateEmployee(department, position, "Two");
        first.Email = null;
        second.Email = null;
        Context.Employees.AddRange(first, second);

        Context.SaveChanges();

        Assert.AreEqual(2, Context.Employees.Count());
    }
}
