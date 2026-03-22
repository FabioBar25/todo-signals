using TodoList.Manager.Contracts;
using TodoList.Manager.Models;

namespace TodoList.Manager.Managers;

public interface ITaskManager
{
    Task<IReadOnlyList<TaskItem>> GetTasksAsync(CancellationToken cancellationToken);

    Task<TaskItem> CreateTaskAsync(SaveTaskRequest request, CancellationToken cancellationToken);

    Task<TaskItem?> UpdateTaskAsync(int id, SaveTaskRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteTaskAsync(int id, CancellationToken cancellationToken);
}
