using NSubstitute;
using NUnit.Framework;
using TodoList.Manager.Access;
using TodoList.Manager.Contracts;
using TodoList.Manager.Managers;
using TodoList.Manager.Models;

namespace TodoList.Manager.Tests;

[TestFixture]
public sealed class TaskManagerTests
{
    private ITaskAccessor taskAccessor = null!;
    private TaskManager sut = null!;

    [SetUp]
    public void SetUp()
    {
        taskAccessor = Substitute.For<ITaskAccessor>();
        sut = new TaskManager(taskAccessor);
    }

    [Test]
    public async Task CreateTaskAsync_WithBlankTitle_ThrowsAndDoesNotCallAccessor()
    {
        var request = new SaveTaskRequest { Title = "   " };

        AsyncTestDelegate act = async () => await sut.CreateTaskAsync(request, CancellationToken.None);

        var exception = Assert.ThrowsAsync<ArgumentException>(act);
        Assert.That(exception!.ParamName, Is.EqualTo("title"));
        await taskAccessor.DidNotReceiveWithAnyArgs().CreateAsync(default!, default);
    }

    [Test]
    public async Task CreateTaskAsync_TrimsTitleBeforeSaving()
    {
        var createdTask = new TaskItem { Id = 7, Title = "Buy milk" };
        taskAccessor.CreateAsync("Buy milk", Arg.Any<CancellationToken>()).Returns(createdTask);

        var result = await sut.CreateTaskAsync(new SaveTaskRequest { Title = "  Buy milk  " }, CancellationToken.None);

        Assert.That(result, Is.EqualTo(createdTask));
        await taskAccessor.Received(1).CreateAsync("Buy milk", Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task UpdateTaskAsync_WithBlankTitle_ThrowsAndDoesNotCallAccessor()
    {
        var request = new SaveTaskRequest { Title = "" };

        AsyncTestDelegate act = async () => await sut.UpdateTaskAsync(12, request, CancellationToken.None);

        var exception = Assert.ThrowsAsync<ArgumentException>(act);
        Assert.That(exception!.ParamName, Is.EqualTo("title"));
        await taskAccessor.DidNotReceiveWithAnyArgs().UpdateAsync(default, default!, default);
    }
}
