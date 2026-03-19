using TodoSignals.Manager.Interfaces;
using TodoSignals.Manager.Models;

namespace TodoSignals.Manager.Services;

public sealed class TaskManager(ITaskAccessor taskAccessor) : ITaskManager
{
    public Task<IReadOnlyList<TaskItem>> GetTasksAsync(CancellationToken cancellationToken = default) =>
        taskAccessor.GetTasksAsync(cancellationToken);

    public Task<TaskItem> CreateTaskAsync(string title, CancellationToken cancellationToken = default)
    {
        var sanitizedTitle = NormalizeTitle(title);
        return taskAccessor.CreateTaskAsync(sanitizedTitle, cancellationToken);
    }

    public Task<TaskItem?> UpdateTaskAsync(int id, string title, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Task id must be greater than zero.");
        }

        var sanitizedTitle = NormalizeTitle(title);
        return taskAccessor.UpdateTaskAsync(id, sanitizedTitle, cancellationToken);
    }

    public Task<bool> DeleteTaskAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Task id must be greater than zero.");
        }

        return taskAccessor.DeleteTaskAsync(id, cancellationToken);
    }

    private static string NormalizeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Task title is required.", nameof(title));
        }

        var normalizedTitle = title.Trim();

        if (normalizedTitle.Length > 200)
        {
            throw new ArgumentException("Task title cannot exceed 200 characters.", nameof(title));
        }

        return normalizedTitle;
    }
}
