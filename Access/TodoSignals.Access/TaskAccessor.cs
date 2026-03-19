using Microsoft.EntityFrameworkCore;
using TodoSignals.Access.Entities;
using TodoSignals.Access.Persistence;
using TodoSignals.Manager.Interfaces;
using TodoSignals.Manager.Models;

namespace TodoSignals.Access;

public sealed class TaskAccessor(TasksDbContext dbContext) : ITaskAccessor
{
    public async Task<IReadOnlyList<TaskItem>> GetTasksAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Tasks
            .AsNoTracking()
            .OrderBy(task => task.Id)
            .Select(task => new TaskItem
            {
                Id = task.Id,
                Title = task.Title
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem> CreateTaskAsync(string title, CancellationToken cancellationToken = default)
    {
        var entity = new TaskEntity
        {
            Title = title
        };

        dbContext.Tasks.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<TaskItem?> UpdateTaskAsync(int id, string title, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Tasks.FirstOrDefaultAsync(task => task.Id == id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        entity.Title = title;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<bool> DeleteTaskAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Tasks.FirstOrDefaultAsync(task => task.Id == id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        dbContext.Tasks.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static TaskItem Map(TaskEntity entity) =>
        new()
        {
            Id = entity.Id,
            Title = entity.Title
        };
}
