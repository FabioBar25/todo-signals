using Microsoft.AspNetCore.Identity;
using TodoList.Manager.Access;
using TodoList.Manager.Contracts;
using TodoList.Manager.Exceptions;
using TodoList.Manager.Models;

namespace TodoList.Manager.Managers;

public sealed class AuthManager(IUserAccessor userAccessor) : IAuthManager
{
    private readonly PasswordHasher<UserAccount> passwordHasher = new();

    public async Task<AuthenticatedUser> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var fullName = NormalizeFullName(request.FullName);
        var email = NormalizeEmail(request.Email);
        var password = NormalizePassword(request.Password);
        var normalizedEmail = email.ToUpperInvariant();

        var existingUser = await userAccessor.GetByNormalizedEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
        {
            throw new DuplicateEmailException("An account with that email already exists.");
        }

        var passwordHash = passwordHasher.HashPassword(
            new UserAccount
            {
                Id = 0,
                FullName = fullName,
                Email = email,
                NormalizedEmail = normalizedEmail,
                PasswordHash = string.Empty
            },
            password);

        var createdUser = await userAccessor.CreateAsync(
            fullName,
            email,
            normalizedEmail,
            passwordHash,
            cancellationToken);

        return Map(createdUser);
    }

    public async Task<AuthenticatedUser> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var email = NormalizeEmail(request.Email);
        var password = NormalizePassword(request.Password);
        var normalizedEmail = email.ToUpperInvariant();

        var user = await userAccessor.GetByNormalizedEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            throw new AuthenticationException("Your email or password looks incorrect.");
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verificationResult is PasswordVerificationResult.Failed)
        {
            throw new AuthenticationException("Your email or password looks incorrect.");
        }

        return Map(user);
    }

    public async Task<AuthenticatedUser?> GetUserAsync(int id, CancellationToken cancellationToken)
    {
        var user = await userAccessor.GetByIdAsync(id, cancellationToken);
        return user is null ? null : Map(user);
    }

    private static AuthenticatedUser Map(UserAccount user) =>
        new()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email
        };

    private static string NormalizeFullName(string fullName)
    {
        var normalized = fullName.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Full name is required.", nameof(fullName));
        }

        return normalized;
    }

    private static string NormalizeEmail(string email)
    {
        var normalized = email.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        return normalized;
    }

    private static string NormalizePassword(string password)
    {
        var normalized = password.Trim();
        if (normalized.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters long.", nameof(password));
        }

        return normalized;
    }
}
