using EmployeeManagement.Api.Contracts;
using EmployeeManagement.Data.Entities;
using EmployeeManagement.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IRepository<Employee> _employees;
    private readonly IRepository<Department> _departments;
    private readonly IRepository<Position> _positions;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(
        IRepository<Employee> employees,
        IRepository<Department> departments,
        IRepository<Position> positions,
        ILogger<EmployeesController> logger)
    {
        _logger = logger;
        _employees = employees;
        _departments = departments;
        _positions = positions;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _employees.GetAllAsync(cancellationToken);
        return Ok(employees.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        return employee is null ? NotFound() : Ok(ToDto(employee));
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(EmployeeRequest request, CancellationToken cancellationToken)
    {
        if (!await ValidateAsync(request, null, cancellationToken))
        {
            _logger.LogWarning("Create employee rejected: validation failed");
            return ValidationProblem(ModelState);
        }

        var employee = new Employee();
        Apply(request, employee);
        await _employees.AddAsync(employee, cancellationToken);
        await _employees.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created employee {EmployeeId}", employee.Id);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, ToDto(employee));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> Update(int id, EmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        if (!await ValidateAsync(request, id, cancellationToken))
        {
            _logger.LogWarning("Update employee {EmployeeId} rejected: validation failed", id);
            return ValidationProblem(ModelState);
        }

        Apply(request, employee);
        _employees.Update(employee);
        await _employees.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Updated employee {EmployeeId}", id);

        return Ok(ToDto(employee));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        _employees.Remove(employee);
        await _employees.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Soft-deleted employee {EmployeeId}", id);

        return NoContent();
    }

    private async Task<bool> ValidateAsync(EmployeeRequest request, int? existingId, CancellationToken cancellationToken)
    {
        if (await _departments.GetByIdAsync(request.DepartmentId, cancellationToken) is null)
        {
            ModelState.AddModelError(nameof(request.DepartmentId), "Department does not exist.");
        }

        if (await _positions.GetByIdAsync(request.PositionId, cancellationToken) is null)
        {
            ModelState.AddModelError(nameof(request.PositionId), "Position does not exist.");
        }

        var email = NormalizeEmail(request.Email);
        if (email is not null && !System.Net.Mail.MailAddress.TryCreate(email, out _))
        {
            ModelState.AddModelError(nameof(request.Email), "Email is not valid.");
        }
        else if (email is not null)
        {
            var id = existingId ?? 0;
            var duplicates = await _employees.FindAsync(e => e.Email == email && e.Id != id, cancellationToken);
            if (duplicates.Count > 0)
            {
                ModelState.AddModelError(nameof(request.Email), "This email is already used by another employee.");
            }
        }

        return ModelState.IsValid;
    }

    private static string? NormalizeEmail(string? email) =>
        string.IsNullOrWhiteSpace(email) ? null : email.Trim();

    private static void Apply(EmployeeRequest request, Employee employee)
    {
        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = NormalizeEmail(request.Email);
        employee.HireDate = request.HireDate;
        employee.Salary = request.Salary;
        employee.DepartmentId = request.DepartmentId;
        employee.PositionId = request.PositionId;
    }

    private static EmployeeDto ToDto(Employee e) =>
        new(e.Id, e.FirstName, e.LastName, e.Email, e.HireDate, e.Salary, e.DepartmentId, e.PositionId);
}
