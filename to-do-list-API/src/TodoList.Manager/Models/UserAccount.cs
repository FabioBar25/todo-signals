namespace TodoList.Manager.Models;

public sealed class UserAccount
{
    public int Id { get; init; }

    public required string FullName { get; init; }

    public required string Email { get; init; }

    public required string NormalizedEmail { get; init; }

    public required string PasswordHash { get; init; }
}
