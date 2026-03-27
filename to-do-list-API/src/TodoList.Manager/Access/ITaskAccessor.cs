using TodoList.Manager.Models;

namespace TodoList.Manager.Access;

public interface ITaskAccessor
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(int userId, CancellationToken cancellationToken);

    Task<TaskItem> CreateAsync(int userId, string title, CancellationToken cancellationToken);

    Task<TaskItem?> UpdateAsync(int userId, int id, string title, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int userId, int id, CancellationToken cancellationToken);
}
