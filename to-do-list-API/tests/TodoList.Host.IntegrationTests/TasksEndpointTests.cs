using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using NUnit.Framework;
using TodoList.Host.IntegrationTests.Testing;

namespace TodoList.Host.IntegrationTests;

[TestFixture]
public sealed class TasksEndpointTests
{
    private DistributedApplication appHost = null!;
    private TaskApiFactory factory = null!;
    private HttpClient client = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Environment.SetEnvironmentVariable("Logging__EventLog__LogLevel__Default", "None");
        Environment.SetEnvironmentVariable("Logging__EventLog__LogLevel__Microsoft", "None");

        var appHostAssembly = LoadAppHostAssembly();
        var entryPointType = appHostAssembly.GetType("Program")
            ?? throw new InvalidOperationException("Could not find the AppHost entry point type.");

        try
        {
            var appHostBuilder = await DistributedApplicationTestingBuilder.CreateAsync(entryPointType);
            appHost = await appHostBuilder.BuildAsync();
            await appHost.StartAsync();
        }
        catch (DistributedApplicationException exception) when (exception.Message.Contains("Container runtime 'docker' was found but appears to be unhealthy.", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Ignore("Docker is not healthy, so the Aspire-backed integration test cannot run in this environment.");
        }

        var connectionString = await appHost.GetConnectionStringAsync("tasksdb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Aspire did not provide a connection string for 'tasksdb'.");
        }

        Environment.SetEnvironmentVariable("ConnectionStrings__tasksdb", connectionString);

        factory = new TaskApiFactory(connectionString);
        client = factory.CreateClient();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        client?.Dispose();

        if (factory is not null)
        {
            await factory.DisposeAsync();
        }

        if (appHost is not null)
        {
            await appHost.DisposeAsync();
        }

        Environment.SetEnvironmentVariable("ConnectionStrings__tasksdb", null);
    }

    [Test]
    public async Task CreateTask_HappyPath_PersistsTaskAndReturnsCreated()
    {
        var response = await client.PostAsJsonAsync("/api/tasks", new SaveTaskRequestDto { Title = "Integration happy path" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var createdTask = await response.Content.ReadFromJsonAsync<TaskItemDto>();
        Assert.That(createdTask, Is.Not.Null);
        Assert.That(createdTask!.Id, Is.GreaterThan(0));
        Assert.That(createdTask.Title, Is.EqualTo("Integration happy path"));
        Assert.That(response.Headers.Location?.ToString(), Is.EqualTo($"/api/tasks/{createdTask.Id}"));

        var tasks = await client.GetFromJsonAsync<List<TaskItemDto>>("/api/tasks");

        Assert.That(tasks, Is.Not.Null);
        Assert.That(tasks!, Has.Count.GreaterThanOrEqualTo(1));
        Assert.That(tasks.Any(task => task.Id == createdTask.Id && task.Title == "Integration happy path"), Is.True);
    }

    private static Assembly LoadAppHostAssembly()
    {
        var repositoryRoot = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "..", ".."));
        var appHostAssemblyPath = Path.Combine(repositoryRoot, "src", "TodoList.AppHost", "bin", "Debug", "net9.0", "TodoList.AppHost.dll");

        if (!File.Exists(appHostAssemblyPath))
        {
            throw new FileNotFoundException($"Expected the AppHost assembly at '{appHostAssemblyPath}'.");
        }

        return Assembly.LoadFrom(appHostAssemblyPath);
    }

    private sealed record SaveTaskRequestDto
    {
        [JsonPropertyName("title")]
        public string Title { get; init; } = string.Empty;
    }

    private sealed record TaskItemDto
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("title")]
        public string Title { get; init; } = string.Empty;
    }
}
