using EmployeeManagement.Data.Entities;
using EmployeeManagement.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data.Tests;

[TestClass]
public class RepositoryTests : DatabaseTestBase
{
    private IRepository<Employee> CreateRepository() => new Repository<Employee>(Context);

    [TestMethod]
    public async Task AddAsync_WithSaveChanges_PersistsEmployee()
    {
        var (department, position) = SeedLookups();
        var repository = CreateRepository();

        await repository.AddAsync(CreateEmployee(department, position));
        await repository.SaveChangesAsync();

        Assert.AreEqual(1, Context.Employees.Count());
    }

    [TestMethod]
    public async Task GetByIdAsync_ExistingId_ReturnsEmployee()
    {
        var (department, position) = SeedLookups();
        var employee = CreateEmployee(department, position);
        Context.Employees.Add(employee);
        Context.SaveChanges();

        var result = await CreateRepository().GetByIdAsync(employee.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual("Sample", result.LastName);
    }

    [TestMethod]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        var result = await CreateRepository().GetByIdAsync(999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllEmployees()
    {
        var (department, position) = SeedLookups();
        Context.Employees.AddRange(
            CreateEmployee(department, position, "One"),
            CreateEmployee(department, position, "Two"));
        Context.SaveChanges();

        var result = await CreateRepository().GetAllAsync();

        Assert.HasCount(2, result);
    }

    [TestMethod]
    public async Task FindAsync_WithPredicate_ReturnsOnlyMatches()
    {
        var (department, position) = SeedLookups();
        Context.Employees.AddRange(
            CreateEmployee(department, position, "One"),
            CreateEmployee(department, position, "Two"));
        Context.SaveChanges();

        var result = await CreateRepository().FindAsync(e => e.LastName == "Two");

        Assert.HasCount(1, result);
        Assert.AreEqual("Two", result[0].LastName);
    }

    [TestMethod]
    public async Task Update_ChangedEmployee_PersistsChanges()
    {
        var (department, position) = SeedLookups();
        var employee = CreateEmployee(department, position);
        Context.Employees.Add(employee);
        Context.SaveChanges();
        var repository = CreateRepository();

        employee.Salary = 60000m;
        repository.Update(employee);
        await repository.SaveChangesAsync();

        Context.ChangeTracker.Clear();
        Assert.AreEqual(60000m, Context.Employees.Single().Salary);
    }

    [TestMethod]
    public async Task Remove_ExistingEmployee_HidesEmployeeFromQueries()
    {
        var (department, position) = SeedLookups();
        var employee = CreateEmployee(department, position);
        Context.Employees.Add(employee);
        Context.SaveChanges();
        var repository = CreateRepository();

        repository.Remove(employee);
        await repository.SaveChangesAsync();

        Assert.AreEqual(0, Context.Employees.Count());
    }

    [TestMethod]
    public async Task Remove_ExistingEmployee_KeepsRowAndSetsIsDeleted()
    {
        var (department, position) = SeedLookups();
        var employee = CreateEmployee(department, position);
        Context.Employees.Add(employee);
        Context.SaveChanges();
        var repository = CreateRepository();

        repository.Remove(employee);
        await repository.SaveChangesAsync();

        Context.ChangeTracker.Clear();
        var row = Context.Employees.IgnoreQueryFilters().Single();
        Assert.IsTrue(row.IsDeleted);
    }

    [TestMethod]
    public async Task GetByIdAsync_SoftDeletedEmployee_ReturnsNull()
    {
        var (department, position) = SeedLookups();
        var employee = CreateEmployee(department, position);
        Context.Employees.Add(employee);
        Context.SaveChanges();
        var repository = CreateRepository();
        repository.Remove(employee);
        await repository.SaveChangesAsync();

        var result = await repository.GetByIdAsync(employee.Id);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetAllAsync_ExcludesSoftDeletedEmployees()
    {
        var (department, position) = SeedLookups();
        var kept = CreateEmployee(department, position, "Kept");
        var removed = CreateEmployee(department, position, "Removed");
        Context.Employees.AddRange(kept, removed);
        Context.SaveChanges();
        var repository = CreateRepository();
        repository.Remove(removed);
        await repository.SaveChangesAsync();

        var result = await repository.GetAllAsync();

        Assert.HasCount(1, result);
        Assert.AreEqual(kept.Id, result[0].Id);
    }

    [TestMethod]
    public async Task GetDeletedAsync_ReturnsOnlySoftDeletedEmployees()
    {
        var (department, position) = SeedLookups();
        var kept = CreateEmployee(department, position, "Kept");
        var removed = CreateEmployee(department, position, "Removed");
        Context.Employees.AddRange(kept, removed);
        Context.SaveChanges();
        var repository = CreateRepository();
        repository.Remove(removed);
        await repository.SaveChangesAsync();

        var result = await repository.GetDeletedAsync();

        Assert.HasCount(1, result);
        Assert.AreEqual(removed.Id, result[0].Id);
        Assert.IsTrue(result[0].IsDeleted);
    }

    [TestMethod]
    public async Task GetAllIncludingDeletedAsync_ReturnsActiveAndDeletedEmployees()
    {
        var (department, position) = SeedLookups();
        var kept = CreateEmployee(department, position, "Kept");
        var removed = CreateEmployee(department, position, "Removed");
        Context.Employees.AddRange(kept, removed);
        Context.SaveChanges();
        var repository = CreateRepository();
        repository.Remove(removed);
        await repository.SaveChangesAsync();

        var result = await repository.GetAllIncludingDeletedAsync();

        Assert.HasCount(2, result);
        Assert.IsTrue(result.Any(e => e.Id == kept.Id && !e.IsDeleted));
        Assert.IsTrue(result.Any(e => e.Id == removed.Id && e.IsDeleted));
    }

    [TestMethod]
    public async Task GetDeletedAsync_NoDeletedEmployees_ReturnsEmpty()
    {
        var (department, position) = SeedLookups();
        Context.Employees.Add(CreateEmployee(department, position));
        Context.SaveChanges();

        var result = await CreateRepository().GetDeletedAsync();

        Assert.IsEmpty(result);
    }
}
