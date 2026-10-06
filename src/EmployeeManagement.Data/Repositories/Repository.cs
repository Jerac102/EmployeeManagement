using System.Linq.Expressions;
using EmployeeManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class, ISoftDeletable
{
    protected readonly EmployeeDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(EmployeeDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, cancellationToken);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task<IReadOnlyList<T>> GetDeletedAsync(CancellationToken cancellationToken = default)
        => await DbSet.IgnoreQueryFilters().AsNoTracking().Where(e => e.IsDeleted).ToListAsync(cancellationToken);

    public virtual async Task<IReadOnlyList<T>> GetAllIncludingDeletedAsync(CancellationToken cancellationToken = default)
        => await DbSet.IgnoreQueryFilters().AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(entity, cancellationToken);

    public virtual void Update(T entity) => DbSet.Update(entity);

    public virtual void Remove(T entity)
    {
        entity.IsDeleted = true;
        DbSet.Update(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => Context.SaveChangesAsync(cancellationToken);
}
