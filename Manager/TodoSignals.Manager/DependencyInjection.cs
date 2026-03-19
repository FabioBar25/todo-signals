using Microsoft.Extensions.DependencyInjection;
using TodoSignals.Manager.Interfaces;
using TodoSignals.Manager.Services;

namespace TodoSignals.Manager;

public static class DependencyInjection
{
    public static IServiceCollection AddManager(this IServiceCollection services)
    {
        services.AddScoped<ITaskManager, TaskManager>();

        return services;
    }
}
