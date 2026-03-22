using Microsoft.Extensions.DependencyInjection;
using TodoList.Manager.Managers;

namespace TodoList.Manager;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddManagerLayer(this IServiceCollection services)
    {
        services.AddScoped<ITaskManager, TaskManager>();
        return services;
    }
}
