namespace TodoList.Manager.Models;

public sealed class TaskItem
{
    public int Id { get; init; }

    public required string Title { get; init; }
}
