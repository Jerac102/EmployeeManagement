using System.Net;
using System.Net.Http.Json;
using EmployeeManagement.Api.Contracts;

namespace EmployeeManagement.Api.Tests;

[TestClass]
public class EmployeesApiTests
{
    private ApiFactory _factory = null!;
    private HttpClient _client = null!;

    [TestInitialize]
    public void SetUp()
    {
        _factory = new ApiFactory();
        _client = _factory.CreateClient();
        _factory.ResetDatabase();
    }

    [TestCleanup]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private EmployeeRequest NewRequest(string lastName = "Sample", string? email = null) => new()
    {
        FirstName = "Max",
        LastName = lastName,
        Email = email ?? $"{lastName.ToLowerInvariant()}@example.com",
        HireDate = new DateOnly(2024, 1, 15),
        Salary = 50000m,
        DepartmentId = _factory.DepartmentId,
        PositionId = _factory.PositionId
    };

    private async Task<EmployeeDto> CreateAsync(EmployeeRequest request)
    {
        var response = await _client.PostAsJsonAsync("/api/employees", request);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<EmployeeDto>())!;
    }

    [TestMethod]
    public async Task Create_ValidRequest_ReturnsCreatedWithLocation()
    {
        var response = await _client.PostAsJsonAsync("/api/employees", NewRequest());

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(response.Headers.Location);
        var dto = await response.Content.ReadFromJsonAsync<EmployeeDto>();
        Assert.IsTrue(dto!.Id > 0);
        Assert.AreEqual(new DateOnly(2024, 1, 15), dto.HireDate);
    }

    [TestMethod]
    public async Task GetAll_ReturnsCreatedEmployees()
    {
        await CreateAsync(NewRequest("One"));
        await CreateAsync(NewRequest("Two"));

        var list = await _client.GetFromJsonAsync<List<EmployeeDto>>("/api/employees");

        Assert.AreEqual(2, list!.Count);
    }

    [TestMethod]
    public async Task GetById_Existing_ReturnsEmployee()
    {
        var created = await CreateAsync(NewRequest());

        var dto = await _client.GetFromJsonAsync<EmployeeDto>($"/api/employees/{created.Id}");

        Assert.AreEqual("Sample", dto!.LastName);
    }

    [TestMethod]
    public async Task GetById_Unknown_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/employees/9999");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task Update_Existing_ChangesValues()
    {
        var created = await CreateAsync(NewRequest());
        var request = NewRequest();
        request.Salary = 70000m;
        request.FirstName = "Anna";

        var response = await _client.PutAsJsonAsync($"/api/employees/{created.Id}", request);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var dto = await _client.GetFromJsonAsync<EmployeeDto>($"/api/employees/{created.Id}");
        Assert.AreEqual(70000m, dto!.Salary);
        Assert.AreEqual("Anna", dto.FirstName);
    }

    [TestMethod]
    public async Task Update_KeepingOwnEmail_Succeeds()
    {
        var created = await CreateAsync(NewRequest());

        var response = await _client.PutAsJsonAsync($"/api/employees/{created.Id}", NewRequest());

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task Update_Unknown_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync("/api/employees/9999", NewRequest());

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task Delete_Existing_SoftDeletesAndHidesEmployee()
    {
        var created = await CreateAsync(NewRequest());

        var response = await _client.DeleteAsync($"/api/employees/{created.Id}");

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        Assert.AreEqual(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/employees/{created.Id}")).StatusCode);
        var list = await _client.GetFromJsonAsync<List<EmployeeDto>>("/api/employees");
        Assert.AreEqual(0, list!.Count);

        var stored = _factory.ReadAllEmployees();
        Assert.AreEqual(1, stored.Count);
        Assert.IsTrue(stored[0].IsDeleted);
    }

    [TestMethod]
    public async Task Delete_Unknown_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/employees/9999");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task Delete_AlreadyDeleted_ReturnsNotFound()
    {
        var created = await CreateAsync(NewRequest());
        await _client.DeleteAsync($"/api/employees/{created.Id}");

        var response = await _client.DeleteAsync($"/api/employees/{created.Id}");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task Create_DuplicateEmail_ReturnsBadRequest()
    {
        await CreateAsync(NewRequest("One", "same@example.com"));

        var response = await _client.PostAsJsonAsync("/api/employees", NewRequest("Two", "same@example.com"));

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task Update_EmailOfAnotherEmployee_ReturnsBadRequest()
    {
        await CreateAsync(NewRequest("One", "one@example.com"));
        var second = await CreateAsync(NewRequest("Two", "two@example.com"));

        var response = await _client.PutAsJsonAsync($"/api/employees/{second.Id}", NewRequest("Two", "one@example.com"));

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task Create_EmptyEmail_Succeeds()
    {
        var request = NewRequest();
        request.Email = "";

        var response = await _client.PostAsJsonAsync("/api/employees", request);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<EmployeeDto>();
        Assert.IsNull(dto!.Email);
    }

    [TestMethod]
    public async Task Create_InvalidEmail_ReturnsBadRequest()
    {
        var request = NewRequest();
        request.Email = "not-an-email";

        var response = await _client.PostAsJsonAsync("/api/employees", request);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task Create_MissingFirstName_ReturnsBadRequest()
    {
        var request = NewRequest();
        request.FirstName = "";

        var response = await _client.PostAsJsonAsync("/api/employees", request);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual(0, _factory.ReadAllEmployees().Count);
    }

    [TestMethod]
    public async Task Create_MissingLastName_ReturnsBadRequest()
    {
        var request = NewRequest();
        request.LastName = "";

        var response = await _client.PostAsJsonAsync("/api/employees", request);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task Create_NegativeSalary_ReturnsBadRequest()
    {
        var request = NewRequest();
        request.Salary = -1m;

        var response = await _client.PostAsJsonAsync("/api/employees", request);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task Create_UnknownDepartment_ReturnsBadRequest()
    {
        var request = NewRequest();
        request.DepartmentId = 9999;

        var response = await _client.PostAsJsonAsync("/api/employees", request);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task Create_UnknownPosition_ReturnsBadRequest()
    {
        var request = NewRequest();
        request.PositionId = 9999;

        var response = await _client.PostAsJsonAsync("/api/employees", request);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task Departments_ReturnsLookups()
    {
        var list = await _client.GetFromJsonAsync<List<LookupDto>>("/api/departments");

        Assert.AreEqual(1, list!.Count);
        Assert.AreEqual("IT", list[0].Name);
    }

    [TestMethod]
    public async Task Positions_ReturnsLookupsWithTitleAsName()
    {
        var list = await _client.GetFromJsonAsync<List<LookupDto>>("/api/positions");

        Assert.AreEqual(1, list!.Count);
        Assert.AreEqual("Developer", list[0].Name);
    }
}
