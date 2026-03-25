using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TodoList.Manager.Access;

namespace TodoList.Access;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = ResolveConnectionString(configuration);

        services.AddDbContext<TodoDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<ITaskAccessor, TaskAccessor>();

        return services;
    }

    private static string ResolveConnectionString(IConfiguration configuration)
    {
        var configuredConnectionString = configuration.GetConnectionString("tasksdb");
        if (!string.IsNullOrWhiteSpace(configuredConnectionString))
        {
            return configuredConnectionString;
        }

        var databaseUrl = configuration["DATABASE_URL"] ?? configuration["POSTGRES_URL"];
        if (!string.IsNullOrWhiteSpace(databaseUrl))
        {
            return TryConvertDatabaseUrl(databaseUrl);
        }

        throw new InvalidOperationException(
            "Database connection is not configured. Set ConnectionStrings__tasksdb or DATABASE_URL.");
    }

    private static string TryConvertDatabaseUrl(string databaseUrl)
    {
        if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri))
        {
            return databaseUrl;
        }

        if (!string.Equals(uri.Scheme, "postgres", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(uri.Scheme, "postgresql", StringComparison.OrdinalIgnoreCase))
        {
            return databaseUrl;
        }

        var userInfoParts = uri.UserInfo.Split(':', 2);
        var username = Uri.UnescapeDataString(userInfoParts[0]);
        var password = userInfoParts.Length > 1 ? Uri.UnescapeDataString(userInfoParts[1]) : string.Empty;

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Database = uri.AbsolutePath.Trim('/'),
            Username = username,
            Password = password,
            SslMode = SslMode.Require
        };

        var query = ParseQueryString(uri.Query);
        if (query.TryGetValue("sslmode", out var sslMode) &&
            Enum.TryParse<SslMode>(sslMode, true, out var parsedSslMode))
        {
            builder.SslMode = parsedSslMode;
        }

        return builder.ConnectionString;
    }

    private static Dictionary<string, string> ParseQueryString(string queryString)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(queryString))
        {
            return values;
        }

        foreach (var pair in queryString.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var keyValue = pair.Split('=', 2);
            var key = Uri.UnescapeDataString(keyValue[0]);
            var value = keyValue.Length > 1 ? Uri.UnescapeDataString(keyValue[1]) : string.Empty;
            values[key] = value;
        }

        return values;
    }
}
