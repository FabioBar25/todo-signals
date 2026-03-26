using Microsoft.EntityFrameworkCore;
using Npgsql;
using TodoList.Access;

namespace TodoList.Host.Infrastructure;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseInitialization");

        const int maxAttempts = 8;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await dbContext.Database.EnsureCreatedAsync();
                return;
            }
            catch (Exception ex) when (IsTransient(ex) && attempt < maxAttempts)
            {
                logger.LogWarning(
                    ex,
                    "Database initialization attempt {Attempt} of {MaxAttempts} failed. Retrying in 2 seconds.",
                    attempt,
                    maxAttempts);

                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        await dbContext.Database.EnsureCreatedAsync();
    }

    private static bool IsTransient(Exception exception) =>
        exception is TimeoutException
        || exception is NpgsqlException
        || exception is InvalidOperationException
        || exception is DbUpdateException;
}
