using Domain.Entities;

namespace Application.Contracts;

public interface IUploadRepository
{
    Task<UploadRecord> CreateAsync(UploadRecord upload, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(string id, UploadStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UploadRecord>> GetPendingAsync(CancellationToken cancellationToken = default);
}
