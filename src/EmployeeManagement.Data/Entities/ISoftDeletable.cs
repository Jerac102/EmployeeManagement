namespace EmployeeManagement.Data.Entities;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
