using Microsoft.EntityFrameworkCore;
using TodoList.Access.Entities;
using TodoList.Manager.Access;
using TodoList.Manager.Models;

namespace TodoList.Access;

public sealed class UserAccessor(TodoDbContext dbContext) : IUserAccessor
{
    public async Task<UserAccount?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == id)
            .Select(Project())
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<UserAccount?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(user => user.NormalizedEmail == normalizedEmail)
            .Select(Project())
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<UserAccount> CreateAsync(
        string fullName,
        string email,
        string normalizedEmail,
        string passwordHash,
        CancellationToken cancellationToken)
    {
        var userRecord = new UserRecord
        {
            FullName = fullName,
            Email = email,
            NormalizedEmail = normalizedEmail,
            PasswordHash = passwordHash,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Users.Add(userRecord);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UserAccount
        {
            Id = userRecord.Id,
            FullName = userRecord.FullName,
            Email = userRecord.Email,
            NormalizedEmail = userRecord.NormalizedEmail,
            PasswordHash = userRecord.PasswordHash
        };
    }

    private static System.Linq.Expressions.Expression<Func<UserRecord, UserAccount>> Project() =>
        user => new UserAccount
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            NormalizedEmail = user.NormalizedEmail,
            PasswordHash = user.PasswordHash
        };
}
