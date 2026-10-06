namespace EmployeeManagement.Data.Entities;

public class Employee : ISoftDeletable
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int PositionId { get; set; }
    public Position? Position { get; set; }
}
