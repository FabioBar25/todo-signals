namespace TodoList.Access.Entities;

public sealed class TaskRecord
{
    public int Id { get; set; }

    public required string Title { get; set; }
}
