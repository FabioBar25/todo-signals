using TodoSignals.Manager.Models;

namespace TodoSignals.Manager.Interfaces;

public interface ITaskManager
{
    Task<IReadOnlyList<TaskItem>> GetTasksAsync(CancellationToken cancellationToken = default);

    Task<TaskItem> CreateTaskAsync(string title, CancellationToken cancellationToken = default);

    Task<TaskItem?> UpdateTaskAsync(int id, string title, CancellationToken cancellationToken = default);

    Task<bool> DeleteTaskAsync(int id, CancellationToken cancellationToken = default);
}
