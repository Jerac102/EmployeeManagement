using System.ComponentModel;
using System.Net.Mail;
using EmployeeManagement.Data.Entities;
using EmployeeManagement.Data.Repositories;
using EmployeeManagement.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EmployeeManagement.Shared.Services;

public class EmployeeEditor : IEmployeeEditor
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ILogger _logger;

    public EmployeeEditor(IServiceScopeFactory scopeFactory, ILogger<EmployeeEditor>? logger = null)
    {
        _scopeFactory = scopeFactory;
        _logger = logger ?? NullLogger<EmployeeEditor>.Instance;
    }

    public BindingList<EmployeeRow> Rows { get; } = new();

    public IReadOnlyList<LookupItem> Departments { get; private set; } = [];

    public IReadOnlyList<LookupItem> Positions { get; private set; } = [];

    public int PendingDeleteCount => Rows.Count(r => r.IsMarkedForDelete);

    public bool HasChanges => Rows.Any(r => r.HasChanges);

    public int ChangedRowCount => Rows.Count(r => r.HasChanges);

    public void MarkForDelete(EmployeeRow row)
    {
        if (row.IsNew)
        {
            Rows.Remove(row);
            return;
        }

        row.IsMarkedForDelete = !row.IsMarkedForDelete;
    }

    public async Task<SaveResult> CommitDeletesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var row in Rows.Where(r => r.IsMarkedForDelete).ToList())
        {
            var result = await DeleteAsync(row, cancellationToken);
            if (!result.Success)
            {
                return result;
            }

            Rows.Remove(row);
        }

        return SaveResult.Ok();
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var departments = await services.GetRequiredService<IRepository<Department>>().GetAllAsync(cancellationToken);
        var positions = await services.GetRequiredService<IRepository<Position>>().GetAllAsync(cancellationToken);
        var employees = await services.GetRequiredService<IRepository<Employee>>().GetAllAsync(cancellationToken);

        Departments = departments.Select(d => new LookupItem(d.Id, d.Name)).ToList();
        Positions = positions.Select(p => new LookupItem(p.Id, p.Title)).ToList();

        Rows.Clear();
        foreach (var employee in employees)
        {
            var row = new EmployeeRow
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                HireDate = employee.HireDate,
                Salary = employee.Salary,
                DepartmentId = employee.DepartmentId,
                PositionId = employee.PositionId
            };
            row.AcceptChanges();
            Rows.Add(row);
        }

        _logger.LogInformation("Loaded {EmployeeCount} employees", Rows.Count);
    }

    public async Task<SaveResult> SaveAsync(EmployeeRow row, CancellationToken cancellationToken = default)
    {
        var error = Validate(row);
        if (error is not null)
        {
            _logger.LogWarning("Validation failed for employee {EmployeeId}: {Error}", row.Id, error);
            return SaveResult.Failure(error);
        }

        var email = string.IsNullOrWhiteSpace(row.Email) ? null : row.Email.Trim();

        await using var scope = _scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<Employee>>();

        if (email is not null)
        {
            var id = row.Id;
            var duplicates = await repository.FindAsync(e => e.Email == email && e.Id != id, cancellationToken);
            if (duplicates.Count > 0)
            {
                return SaveResult.Failure("This email is already used by another employee.");
            }
        }

        if (row.Id == 0)
        {
            var employee = new Employee();
            Map(row, employee, email);
            await repository.AddAsync(employee, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            row.Id = employee.Id;
            row.AcceptChanges();
            _logger.LogInformation("Created employee {EmployeeId}", row.Id);
            return SaveResult.Ok();
        }

        var existing = await repository.GetByIdAsync(row.Id, cancellationToken);
        if (existing is null)
        {
            return SaveResult.Failure("The employee no longer exists.");
        }

        Map(row, existing, email);
        repository.Update(existing);
        await repository.SaveChangesAsync(cancellationToken);
        row.AcceptChanges();
        _logger.LogInformation("Updated employee {EmployeeId}", row.Id);
        return SaveResult.Ok();
    }

    public async Task<SaveResult> DeleteAsync(EmployeeRow row, CancellationToken cancellationToken = default)
    {
        if (row.Id == 0)
        {
            return SaveResult.Ok();
        }

        await using var scope = _scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<Employee>>();

        var existing = await repository.GetByIdAsync(row.Id, cancellationToken);
        if (existing is null)
        {
            return SaveResult.Failure("The employee no longer exists.");
        }

        repository.Remove(existing);
        await repository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Soft-deleted employee {EmployeeId}", row.Id);
        return SaveResult.Ok();
    }

    private string? Validate(EmployeeRow row)
    {
        if (string.IsNullOrWhiteSpace(row.FirstName))
        {
            return "First name is required.";
        }

        if (string.IsNullOrWhiteSpace(row.LastName))
        {
            return "Last name is required.";
        }

        if (!string.IsNullOrWhiteSpace(row.Email) && !MailAddress.TryCreate(row.Email.Trim(), out _))
        {
            return "Email is not valid.";
        }

        if (row.Salary < 0)
        {
            return "Salary cannot be negative.";
        }

        if (!Departments.Any(d => d.Id == row.DepartmentId))
        {
            return "Department is required.";
        }

        if (!Positions.Any(p => p.Id == row.PositionId))
        {
            return "Position is required.";
        }

        return null;
    }

    private static void Map(EmployeeRow row, Employee employee, string? email)
    {
        employee.FirstName = row.FirstName.Trim();
        employee.LastName = row.LastName.Trim();
        employee.Email = email;
        employee.HireDate = row.HireDate;
        employee.Salary = row.Salary;
        employee.DepartmentId = row.DepartmentId;
        employee.PositionId = row.PositionId;
    }
}
