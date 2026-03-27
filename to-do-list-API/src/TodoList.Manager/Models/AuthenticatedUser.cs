namespace TodoList.Manager.Models;

public sealed class AuthenticatedUser
{
    public int Id { get; init; }

    public required string FullName { get; init; }

    public required string Email { get; init; }
}
