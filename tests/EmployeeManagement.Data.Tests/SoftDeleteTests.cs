using EmployeeManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data.Tests;

[TestClass]
public class SoftDeleteTests : DatabaseTestBase
{
    [TestMethod]
    public void Query_DeletedEmployee_IsExcluded()
    {
        var (department, position) = SeedLookups();
        var active = CreateEmployee(department, position, "Active");
        var deleted = CreateEmployee(department, position, "Deleted");
        deleted.IsDeleted = true;
        Context.Employees.AddRange(active, deleted);
        Context.SaveChanges();

        var result = Context.Employees.ToList();

        Assert.HasCount(1, result);
        Assert.AreEqual("Active", result[0].LastName);
    }

    [TestMethod]
    public void Query_DeletedEmployee_IsReturnedWhenFilterIgnored()
    {
        var (department, position) = SeedLookups();
        var deleted = CreateEmployee(department, position, "Deleted");
        deleted.IsDeleted = true;
        Context.Employees.Add(deleted);
        Context.SaveChanges();

        var result = Context.Employees.IgnoreQueryFilters().ToList();

        Assert.HasCount(1, result);
        Assert.IsTrue(result[0].IsDeleted);
    }

    [TestMethod]
    public void Query_DeletedDepartment_IsExcluded()
    {
        Context.Departments.Add(new Department { Name = "Active" });
        Context.Departments.Add(new Department { Name = "Deleted", IsDeleted = true });
        Context.SaveChanges();

        var result = Context.Departments.ToList();

        Assert.HasCount(1, result);
        Assert.AreEqual("Active", result[0].Name);
    }

    [TestMethod]
    public void Query_DeletedPosition_IsExcluded()
    {
        Context.Positions.Add(new Position { Title = "Active" });
        Context.Positions.Add(new Position { Title = "Deleted", IsDeleted = true });
        Context.SaveChanges();

        var result = Context.Positions.ToList();

        Assert.HasCount(1, result);
        Assert.AreEqual("Active", result[0].Title);
    }

    [TestMethod]
    public void NewEntity_IsNotDeletedByDefault()
    {
        var (department, position) = SeedLookups();
        var employee = CreateEmployee(department, position);
        Context.Employees.Add(employee);
        Context.SaveChanges();

        Context.ChangeTracker.Clear();

        Assert.IsFalse(Context.Employees.Single().IsDeleted);
    }
}
