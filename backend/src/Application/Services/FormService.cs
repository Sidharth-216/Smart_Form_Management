using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Shared;

namespace Application.Services;

public sealed class FormService(IFormRepository repository, ICacheService cache)
{
    public async Task<PagedResult<FormDto>> GetFormsAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var paged = await repository.GetAsync(query, cancellationToken);
        return new PagedResult<FormDto>(paged.Items.Select(Map).ToList(), paged.Page, paged.PageSize, paged.TotalCount);
    }

    public async Task<FormDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"form:{id}";
        var cached = await cache.GetAsync<FormDto>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var form = await repository.GetByIdAsync(id, cancellationToken);
        if (form is null)
        {
            return null;
        }

        var dto = Map(form);
        await cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5), cancellationToken);
        return dto;
    }

    public async Task<PagedResult<FormDto>> SearchAsync(string query, string? country, string? state, string? department, string? category, PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"forms:search:{query}:{country}:{state}:{department}:{category}:{pagination.SafePage}:{pagination.SafePageSize}".ToLowerInvariant();
        var cached = await cache.GetAsync<PagedResult<FormDto>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var items = await repository.SearchAsync(query, country, state, department, category, pagination.SafePage, pagination.SafePageSize, cancellationToken);
        var total = await repository.CountSearchAsync(query, country, state, department, category, cancellationToken);
        var result = new PagedResult<FormDto>(items.Select(Map).ToList(), pagination.SafePage, pagination.SafePageSize, total);
        await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(3), cancellationToken);
        return result;
    }

    public async Task<FormDto> CreateAsync(FormUpsertRequest request, string uploadedBy, CancellationToken cancellationToken = default)
    {
        var form = new FormDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Country = string.IsNullOrWhiteSpace(request.Country) ? "India" : request.Country.Trim(),
            State = request.State.Trim(),
            Department = request.Department.Trim(),
            Category = request.Category.Trim(),
            Keywords = ParseCsv(request.KeywordsCsv),
            Language = ParseCsv(request.LanguageCsv),
            Version = string.IsNullOrWhiteSpace(request.Version) ? "1.0.0" : request.Version.Trim(),
            IsLatest = true,
            UploadedBy = uploadedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await repository.CreateAsync(form, cancellationToken);
        await repository.SetLatestAsync(created.Department, created.Category, created.Id, cancellationToken);
        await cache.RemoveAsync($"form:{created.Id}", cancellationToken);
        return Map(created);
    }

    public async Task<bool> UpdateAsync(string id, FormUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.Title = request.Title.Trim();
        existing.Description = request.Description.Trim();
        existing.Country = string.IsNullOrWhiteSpace(request.Country) ? "India" : request.Country.Trim();
        existing.State = request.State.Trim();
        existing.Department = request.Department.Trim();
        existing.Category = request.Category.Trim();
        existing.Keywords = ParseCsv(request.KeywordsCsv);
        existing.Language = ParseCsv(request.LanguageCsv);
        existing.Version = string.IsNullOrWhiteSpace(request.Version) ? existing.Version : request.Version.Trim();
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await repository.UpdateAsync(existing, cancellationToken);
        await cache.RemoveAsync($"form:{id}", cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (deleted)
        {
            await cache.RemoveAsync($"form:{id}", cancellationToken);
        }

        return deleted;
    }

    public Task<FormBrowseFacetsDto> GetBrowseFacetsAsync(CancellationToken cancellationToken = default) => repository.GetBrowseFacetsAsync(cancellationToken);

    private static FormDto Map(FormDocument form) => new(
        form.Id,
        form.Title,
        form.Description,
        form.Country,
        form.State,
        form.Department,
        form.Category,
        form.Keywords,
        form.FileUrl,
        form.PreviewUrl,
        form.Language,
        form.Version,
        form.IsLatest,
        form.UploadedBy,
        form.CreatedAt,
        form.UpdatedAt);

    private static List<string> ParseCsv(string? value) => string.IsNullOrWhiteSpace(value)
        ? []
        : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
}
