using EmployeeManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data;

public class EmployeeDbContext : DbContext
{
    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(e =>
        {
            e.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            e.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.Salary).HasPrecision(18, 2);
            e.HasOne(x => x.Department).WithMany(d => d.Employees).HasForeignKey(x => x.DepartmentId);
            e.HasOne(x => x.Position).WithMany(p => p.Employees).HasForeignKey(x => x.PositionId);
        });

        modelBuilder.Entity<Department>().Property(x => x.Name).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Position>().Property(x => x.Title).IsRequired().HasMaxLength(100);

        modelBuilder.Entity<Employee>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Department>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Position>().HasQueryFilter(x => !x.IsDeleted);
    }
}
