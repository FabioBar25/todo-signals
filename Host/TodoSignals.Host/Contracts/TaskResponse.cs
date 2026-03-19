namespace TodoSignals.Host.Contracts;

public sealed class TaskResponse
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;
}
