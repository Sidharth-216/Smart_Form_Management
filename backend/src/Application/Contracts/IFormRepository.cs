using Application.DTOs;
using Domain.Entities;
using Shared;

namespace Application.Contracts;

public interface IFormRepository
{
    Task<PagedResult<FormDocument>> GetAsync(PaginationQuery query, CancellationToken cancellationToken = default);
    Task<FormDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FormDocument>> SearchAsync(string query, string? country, string? state, string? department, string? category, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<long> CountSearchAsync(string query, string? country, string? state, string? department, string? category, CancellationToken cancellationToken = default);
    Task<FormDocument> CreateAsync(FormDocument form, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(FormDocument form, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task SetLatestAsync(string department, string category, string currentId, CancellationToken cancellationToken = default);
    Task<FormBrowseFacetsDto> GetBrowseFacetsAsync(CancellationToken cancellationToken = default);
}
