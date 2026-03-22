namespace TodoList.Manager.Contracts;

public sealed class SaveTaskRequest
{
    public required string Title { get; init; }
}
