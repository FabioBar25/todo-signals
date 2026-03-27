using Microsoft.EntityFrameworkCore;
using TodoList.Access.Entities;
using TodoList.Manager.Access;
using TodoList.Manager.Models;

namespace TodoList.Access;

public sealed class TaskAccessor(TodoDbContext dbContext) : ITaskAccessor
{
    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(int userId, CancellationToken cancellationToken)
    {
        return await dbContext.Tasks
            .AsNoTracking()
            .Where(task => task.UserId == userId)
            .OrderBy(task => task.Id)
            .Select(task => new TaskItem
            {
                Id = task.Id,
                Title = task.Title
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem> CreateAsync(int userId, string title, CancellationToken cancellationToken)
    {
        var taskRecord = new TaskRecord
        {
            UserId = userId,
            Title = title
        };

        dbContext.Tasks.Add(taskRecord);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Map(taskRecord);
    }

    public async Task<TaskItem?> UpdateAsync(int userId, int id, string title, CancellationToken cancellationToken)
    {
        var taskRecord = await dbContext.Tasks.SingleOrDefaultAsync(
            task => task.Id == id && task.UserId == userId,
            cancellationToken);
        if (taskRecord is null)
        {
            return null;
        }

        taskRecord.Title = title;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Map(taskRecord);
    }

    public async Task<bool> DeleteAsync(int userId, int id, CancellationToken cancellationToken)
    {
        var taskRecord = await dbContext.Tasks.SingleOrDefaultAsync(
            task => task.Id == id && task.UserId == userId,
            cancellationToken);
        if (taskRecord is null)
        {
            return false;
        }

        dbContext.Tasks.Remove(taskRecord);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TaskItem Map(TaskRecord taskRecord) =>
        new()
        {
            Id = taskRecord.Id,
            Title = taskRecord.Title
        };
}
