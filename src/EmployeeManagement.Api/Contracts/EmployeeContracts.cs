using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Contracts;

public sealed record EmployeeDto(
    int Id,
    string FirstName,
    string LastName,
    string? Email,
    DateOnly HireDate,
    decimal Salary,
    int DepartmentId,
    int PositionId);

public sealed class EmployeeRequest
{
    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Email { get; set; }

    public DateOnly HireDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Salary { get; set; }

    public int DepartmentId { get; set; }

    public int PositionId { get; set; }
}

public sealed record LookupDto(int Id, string Name);
