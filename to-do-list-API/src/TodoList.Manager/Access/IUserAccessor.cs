using TodoList.Manager.Models;

namespace TodoList.Manager.Access;

public interface IUserAccessor
{
    Task<UserAccount?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<UserAccount?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

    Task<UserAccount> CreateAsync(
        string fullName,
        string email,
        string normalizedEmail,
        string passwordHash,
        CancellationToken cancellationToken);
}
