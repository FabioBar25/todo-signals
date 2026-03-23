using System.Net;
using System.Net.Http.Json;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TodoList.Access;
using TodoList.Host.IntegrationTests.Testing;
using TodoList.Manager.Contracts;
using TodoList.Manager.Models;

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
        appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TodoList_AppHost>();
        await appHost.StartAsync();

        var connectionString = await appHost.GetConnectionStringAsync("tasksdb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Aspire did not provide a connection string for 'tasksdb'.");
        }

        factory = new TaskApiFactory(connectionString);
        client = factory.CreateClient();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        client.Dispose();
        await factory.DisposeAsync();
        await appHost.DisposeAsync();
    }

    [Test]
    public async Task CreateTask_HappyPath_PersistsTaskAndReturnsCreated()
    {
        var response = await client.PostAsJsonAsync("/api/tasks", new SaveTaskRequest { Title = "Integration happy path" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var createdTask = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.That(createdTask, Is.Not.Null);
        Assert.That(createdTask!.Id, Is.GreaterThan(0));
        Assert.That(createdTask.Title, Is.EqualTo("Integration happy path"));
        Assert.That(response.Headers.Location?.ToString(), Is.EqualTo($"/api/tasks/{createdTask.Id}"));

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        var persistedTask = await dbContext.Tasks.FindAsync(createdTask.Id);

        Assert.That(persistedTask, Is.Not.Null);
        Assert.That(persistedTask!.Title, Is.EqualTo("Integration happy path"));
    }
}
