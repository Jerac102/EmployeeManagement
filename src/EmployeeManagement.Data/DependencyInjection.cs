using EmployeeManagement.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EmployeeDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }
}
