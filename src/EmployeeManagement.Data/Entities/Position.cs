namespace EmployeeManagement.Data.Entities;

public class Position : ISoftDeletable
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public string Title { get; set; } = string.Empty;
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
