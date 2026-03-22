using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Manager.Access;

namespace TodoList.Access;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("tasksdb")
            ?? "Host=localhost;Port=5432;Database=tasksdb;Username=postgres;Password=postgres";

        services.AddDbContext<TodoDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<ITaskAccessor, TaskAccessor>();

        return services;
    }
}
