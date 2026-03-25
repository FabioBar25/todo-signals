using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TodoList.Host.Controllers;

namespace TodoList.Host.IntegrationTests.Testing;

internal sealed class TaskApiFactory(string connectionString) : WebApplicationFactory<TasksController>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var repositoryRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var hostProjectPath = Path.Combine(repositoryRoot, "src", "TodoList.Host");

        builder.UseEnvironment("Development");
        builder.UseContentRoot(hostProjectPath);
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:tasksdb"] = connectionString
            });
        });
    }
}
