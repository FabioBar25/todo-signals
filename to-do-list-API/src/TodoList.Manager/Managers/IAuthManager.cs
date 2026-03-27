using TodoList.Manager.Contracts;
using TodoList.Manager.Models;

namespace TodoList.Manager.Managers;

public interface IAuthManager
{
    Task<AuthenticatedUser> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken);

    Task<AuthenticatedUser> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken);

    Task<AuthenticatedUser?> GetUserAsync(int id, CancellationToken cancellationToken);
}
