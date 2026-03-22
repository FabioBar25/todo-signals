using TodoList.Manager.Access;
using TodoList.Manager.Contracts;
using TodoList.Manager.Models;

namespace TodoList.Manager.Managers;

public sealed class TaskManager(ITaskAccessor taskAccessor) : ITaskManager
{
    public Task<IReadOnlyList<TaskItem>> GetTasksAsync(CancellationToken cancellationToken) =>
        taskAccessor.GetAllAsync(cancellationToken);

    public Task<TaskItem> CreateTaskAsync(SaveTaskRequest request, CancellationToken cancellationToken) =>
        taskAccessor.CreateAsync(NormalizeTitle(request.Title), cancellationToken);

    public Task<TaskItem?> UpdateTaskAsync(int id, SaveTaskRequest request, CancellationToken cancellationToken) =>
        taskAccessor.UpdateAsync(id, NormalizeTitle(request.Title), cancellationToken);

    public Task<bool> DeleteTaskAsync(int id, CancellationToken cancellationToken) =>
        taskAccessor.DeleteAsync(id, cancellationToken);

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
