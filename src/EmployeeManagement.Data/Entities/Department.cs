namespace EmployeeManagement.Data.Entities;

public class Department : ISoftDeletable
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
