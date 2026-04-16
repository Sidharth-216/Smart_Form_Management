using Application.Contracts;
using Domain.Entities;
using Infrastructure.Persistence;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public sealed class UploadRepository(MongoDbContext context) : IUploadRepository
{
    public async Task<UploadRecord> CreateAsync(UploadRecord upload, CancellationToken cancellationToken = default)
    {
        await context.Uploads.InsertOneAsync(upload, cancellationToken: cancellationToken);
        return upload;
    }

    public async Task<bool> UpdateStatusAsync(string id, UploadStatus status, CancellationToken cancellationToken = default)
    {
        var update = Builders<UploadRecord>.Update.Set(x => x.Status, status).Set(x => x.UpdatedAt, DateTime.UtcNow);
        var result = await context.Uploads.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<IReadOnlyList<UploadRecord>> GetPendingAsync(CancellationToken cancellationToken = default) =>
        await context.Uploads.Find(x => x.Status == UploadStatus.Pending).SortByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
}
