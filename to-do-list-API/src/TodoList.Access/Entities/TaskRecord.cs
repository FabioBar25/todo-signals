namespace TodoList.Access.Entities;

public sealed class TaskRecord
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public required string Title { get; set; }

    public UserRecord User { get; set; } = null!;
}
