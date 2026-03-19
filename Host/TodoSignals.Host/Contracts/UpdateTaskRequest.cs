namespace TodoSignals.Host.Contracts;

public sealed class UpdateTaskRequest
{
    public string Title { get; init; } = string.Empty;
}
