using Domain.Entities;

namespace Application.Contracts;

public interface IUserRepository
{
    Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserAccount?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<UserAccount> CreateAsync(UserAccount user, CancellationToken cancellationToken = default);
}
