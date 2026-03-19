using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoSignals.Access.Persistence;
using TodoSignals.Manager.Interfaces;

namespace TodoSignals.Access;

public static class DependencyInjection
{
    public static IServiceCollection AddAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TasksDatabase")
            ?? "Data Source=Data/tasks.db";

        services.AddDbContext<TasksDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<ITaskAccessor, TaskAccessor>();

        return services;
    }
}
