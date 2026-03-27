using TodoList.Manager.Contracts;
using TodoList.Manager.Models;

namespace TodoList.Manager.Managers;

public interface ITaskManager
{
    Task<IReadOnlyList<TaskItem>> GetTasksAsync(int userId, CancellationToken cancellationToken);

    Task<TaskItem> CreateTaskAsync(int userId, SaveTaskRequest request, CancellationToken cancellationToken);

    Task<TaskItem?> UpdateTaskAsync(int userId, int id, SaveTaskRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteTaskAsync(int userId, int id, CancellationToken cancellationToken);
}
