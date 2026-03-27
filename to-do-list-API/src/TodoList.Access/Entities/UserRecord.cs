namespace TodoList.Access.Entities;

public sealed class UserRecord
{
    public int Id { get; set; }

    public required string FullName { get; set; }

    public required string Email { get; set; }

    public required string NormalizedEmail { get; set; }

    public required string PasswordHash { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public ICollection<TaskRecord> Tasks { get; set; } = new List<TaskRecord>();
}
