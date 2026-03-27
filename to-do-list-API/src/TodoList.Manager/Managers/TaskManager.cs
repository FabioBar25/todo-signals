using TodoList.Manager.Access;
using TodoList.Manager.Contracts;
using TodoList.Manager.Models;

namespace TodoList.Manager.Managers;

public sealed class TaskManager(ITaskAccessor taskAccessor) : ITaskManager
{
    public Task<IReadOnlyList<TaskItem>> GetTasksAsync(int userId, CancellationToken cancellationToken) =>
        taskAccessor.GetAllAsync(userId, cancellationToken);

    public Task<TaskItem> CreateTaskAsync(int userId, SaveTaskRequest request, CancellationToken cancellationToken) =>
        taskAccessor.CreateAsync(userId, NormalizeTitle(request.Title), cancellationToken);

    public Task<TaskItem?> UpdateTaskAsync(int userId, int id, SaveTaskRequest request, CancellationToken cancellationToken) =>
        taskAccessor.UpdateAsync(userId, id, NormalizeTitle(request.Title), cancellationToken);

    public Task<bool> DeleteTaskAsync(int userId, int id, CancellationToken cancellationToken) =>
        taskAccessor.DeleteAsync(userId, id, cancellationToken);

    private static string NormalizeTitle(string title)
    {
        var normalizedTitle = title.Trim();
        if (string.IsNullOrWhiteSpace(normalizedTitle))
        {
            throw new ArgumentException("Task title is required.", nameof(title));
        }

        return normalizedTitle;
    }
}
