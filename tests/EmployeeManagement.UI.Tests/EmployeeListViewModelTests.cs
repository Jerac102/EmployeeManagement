namespace EmployeeManagement.UI.Tests;

[TestClass]
public class EmployeeListViewModelTests : ViewModelTestBase
{
    [TestMethod]
    public async Task LoadAsync_LoadsLookups()
    {
        await ViewModel.LoadAsync();

        Assert.HasCount(1, ViewModel.Departments);
        Assert.AreEqual("IT", ViewModel.Departments[0].Name);
        Assert.HasCount(1, ViewModel.Positions);
        Assert.AreEqual("Developer", ViewModel.Positions[0].Name);
    }

    [TestMethod]
    public async Task LoadAsync_LoadsEmployeesAsRows()
    {
        SeedEmployee("One");
        SeedEmployee("Two");

        await ViewModel.LoadAsync();

        Assert.HasCount(2, ViewModel.Rows);
        Assert.IsTrue(ViewModel.Rows.All(r => r.Id > 0));
        Assert.IsTrue(ViewModel.Rows.All(r => r.DepartmentId == DepartmentId));
    }

    [TestMethod]
    public async Task LoadAsync_ExcludesSoftDeletedEmployees()
    {
        SeedEmployee("Active");
        SeedEmployee("Deleted", isDeleted: true);

        await ViewModel.LoadAsync();

        Assert.HasCount(1, ViewModel.Rows);
        Assert.AreEqual("Active", ViewModel.Rows[0].LastName);
    }

    [TestMethod]
    public async Task LoadAsync_CalledTwice_DoesNotDuplicateRows()
    {
        SeedEmployee("One");

        await ViewModel.LoadAsync();
        await ViewModel.LoadAsync();

        Assert.HasCount(1, ViewModel.Rows);
    }

    [TestMethod]
    public async Task SaveAsync_NewRow_InsertsEmployeeAndAssignsId()
    {
        await ViewModel.LoadAsync();
        var row = CreateRow();

        var result = await ViewModel.SaveAsync(row);

        Assert.IsTrue(result.Success);
        Assert.IsGreaterThan(0, row.Id);
        Assert.HasCount(1, ReadAllEmployees());
    }

    [TestMethod]
    public async Task SaveAsync_ExistingRow_UpdatesEmployee()
    {
        SeedEmployee("One");
        await ViewModel.LoadAsync();
        var row = ViewModel.Rows[0];

        row.Salary = 60000m;
        row.LastName = "Changed";
        var result = await ViewModel.SaveAsync(row);

        Assert.IsTrue(result.Success);
        var employee = ReadAllEmployees().Single();
        Assert.AreEqual(60000m, employee.Salary);
        Assert.AreEqual("Changed", employee.LastName);
    }

    [TestMethod]
    public async Task SaveAsync_MissingFirstName_ReturnsErrorAndDoesNotSave()
    {
        await ViewModel.LoadAsync();
        var row = CreateRow();
        row.FirstName = " ";

        var result = await ViewModel.SaveAsync(row);

        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsEmpty(ReadAllEmployees());
    }

    [TestMethod]
    public async Task SaveAsync_MissingLastName_ReturnsError()
    {
        await ViewModel.LoadAsync();
        var row = CreateRow();
        row.LastName = string.Empty;

        var result = await ViewModel.SaveAsync(row);

        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public async Task SaveAsync_NegativeSalary_ReturnsError()
    {
        await ViewModel.LoadAsync();
        var row = CreateRow();
        row.Salary = -1m;

        var result = await ViewModel.SaveAsync(row);

        Assert.IsFalse(result.Success);
        Assert.IsEmpty(ReadAllEmployees());
    }

    [TestMethod]
    public async Task SaveAsync_UnknownDepartment_ReturnsError()
    {
        await ViewModel.LoadAsync();
        var row = CreateRow();
        row.DepartmentId = 0;

        var result = await ViewModel.SaveAsync(row);

        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public async Task SaveAsync_UnknownPosition_ReturnsError()
    {
        await ViewModel.LoadAsync();
        var row = CreateRow();
        row.PositionId = 0;

        var result = await ViewModel.SaveAsync(row);

        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public async Task SaveAsync_InvalidEmail_ReturnsError()
    {
        await ViewModel.LoadAsync();
        var row = CreateRow(email: "not-an-email");

        var result = await ViewModel.SaveAsync(row);

        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public async Task SaveAsync_EmptyEmail_Succeeds()
    {
        await ViewModel.LoadAsync();
        var row = CreateRow();
        row.Email = null;

        var result = await ViewModel.SaveAsync(row);

        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public async Task SaveAsync_DuplicateEmail_ReturnsError()
    {
        SeedEmployee("One");
        await ViewModel.LoadAsync();
        var row = CreateRow("Two", email: "one@example.com");

        var result = await ViewModel.SaveAsync(row);

        Assert.IsFalse(result.Success);
        Assert.HasCount(1, ReadAllEmployees());
    }

    [TestMethod]
    public async Task SaveAsync_ExistingRowKeepingOwnEmail_Succeeds()
    {
        SeedEmployee("One");
        await ViewModel.LoadAsync();
        var row = ViewModel.Rows[0];

        row.FirstName = "Changed";
        var result = await ViewModel.SaveAsync(row);

        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public async Task SaveAsync_FailedAdd_DoesNotAffectNextSave()
    {
        SeedEmployee("One");
        await ViewModel.LoadAsync();
        var duplicate = CreateRow("Two", email: "one@example.com");
        await ViewModel.SaveAsync(duplicate);

        var corrected = CreateRow("Three");
        var result = await ViewModel.SaveAsync(corrected);

        Assert.IsTrue(result.Success);
        Assert.HasCount(2, ReadAllEmployees());
    }

    [TestMethod]
    public async Task DeleteAsync_ExistingRow_SoftDeletesEmployee()
    {
        SeedEmployee("One");
        await ViewModel.LoadAsync();

        var result = await ViewModel.DeleteAsync(ViewModel.Rows[0]);

        Assert.IsTrue(result.Success);
        var employee = ReadAllEmployees().Single();
        Assert.IsTrue(employee.IsDeleted);
    }

    [TestMethod]
    public async Task DeleteAsync_UnsavedRow_SucceedsWithoutTouchingDatabase()
    {
        await ViewModel.LoadAsync();

        var result = await ViewModel.DeleteAsync(CreateRow());

        Assert.IsTrue(result.Success);
        Assert.IsEmpty(ReadAllEmployees());
    }

    [TestMethod]
    public async Task DeleteAsync_RowAlreadyDeleted_ReturnsError()
    {
        SeedEmployee("One");
        await ViewModel.LoadAsync();
        var row = ViewModel.Rows[0];
        await ViewModel.DeleteAsync(row);

        var result = await ViewModel.DeleteAsync(row);

        Assert.IsFalse(result.Success);
    }
}
