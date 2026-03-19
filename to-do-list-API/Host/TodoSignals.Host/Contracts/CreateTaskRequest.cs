namespace TodoSignals.Host.Contracts;

public sealed class CreateTaskRequest
{
    public string Title { get; init; } = string.Empty;
}
