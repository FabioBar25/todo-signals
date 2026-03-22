using TodoList.Manager.Models;

namespace TodoList.Manager.Access;

public interface ITaskAccessor
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken);

    Task<TaskItem> CreateAsync(string title, CancellationToken cancellationToken);

    Task<TaskItem?> UpdateAsync(int id, string title, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
