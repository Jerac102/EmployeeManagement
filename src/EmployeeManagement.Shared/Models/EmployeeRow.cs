namespace EmployeeManagement.Shared.Models;

public sealed class EmployeeRow
{
    private Snapshot? _original;

    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateOnly HireDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public decimal Salary { get; set; }
    public int DepartmentId { get; set; }
    public int PositionId { get; set; }

    public bool IsNew => Id == 0;

    public bool IsMarkedForDelete { get; set; }

    public bool IsModified => !IsNew && _original is { } original && original != CreateSnapshot();

    public bool HasChanges => IsMarkedForDelete || (IsNew ? HasUserInput : IsModified);

    public void AcceptChanges() => _original = CreateSnapshot();

    private bool HasUserInput =>
        !string.IsNullOrWhiteSpace(FirstName) ||
        !string.IsNullOrWhiteSpace(LastName) ||
        !string.IsNullOrWhiteSpace(Email) ||
        Salary != 0;

    private Snapshot CreateSnapshot() =>
        new(FirstName, LastName, Email ?? string.Empty, HireDate, Salary, DepartmentId, PositionId);

    private readonly record struct Snapshot(
        string FirstName,
        string LastName,
        string Email,
        DateOnly HireDate,
        decimal Salary,
        int DepartmentId,
        int PositionId);
}
