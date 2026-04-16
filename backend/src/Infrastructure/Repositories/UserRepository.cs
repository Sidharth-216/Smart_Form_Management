using Application.Contracts;
using Domain.Entities;
using Infrastructure.Persistence;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public sealed class UserRepository(MongoDbContext context) : IUserRepository
{
    public async Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await context.Users.Find(x => x.Email == email).FirstOrDefaultAsync(cancellationToken);

    public async Task<UserAccount?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await context.Users.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<UserAccount> CreateAsync(UserAccount user, CancellationToken cancellationToken = default)
    {
        await context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);
        return user;
    }
}
