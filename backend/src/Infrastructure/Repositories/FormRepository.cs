using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Persistence;
using MongoDB.Driver;
using Shared;

namespace Infrastructure.Repositories;

public sealed class FormRepository(MongoDbContext context) : IFormRepository
{
    public async Task<PagedResult<FormDocument>> GetAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.SafePage;
        var pageSize = query.SafePageSize;
        var total = await context.Forms.CountDocumentsAsync(_ => true, cancellationToken: cancellationToken);
        var items = await context.Forms.Find(_ => true).SortByDescending(x => x.IsLatest).ThenByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<FormDocument>(items, page, pageSize, total);
    }

    public async Task<FormDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await context.Forms.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<FormDocument>> SearchAsync(string query, string? country, string? state, string? department, string? category, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var filter = BuildFilter(query, country, state, department, category);
        return await context.Forms.Find(filter).SortByDescending(x => x.IsLatest).ThenByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync(cancellationToken);
    }

    public Task<long> CountSearchAsync(string query, string? country, string? state, string? department, string? category, CancellationToken cancellationToken = default)
    {
        var filter = BuildFilter(query, country, state, department, category);
        return context.Forms.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }

    public async Task<FormDocument> CreateAsync(FormDocument form, CancellationToken cancellationToken = default)
    {
        await context.Forms.InsertOneAsync(form, cancellationToken: cancellationToken);
        return form;
    }

    public Task<bool> UpdateAsync(FormDocument form, CancellationToken cancellationToken = default)
    {
        var replace = context.Forms.ReplaceOneAsync(x => x.Id == form.Id, form, cancellationToken: cancellationToken);
        return replace.ContinueWith(x => x.Result.IsAcknowledged && x.Result.ModifiedCount > 0, cancellationToken);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        context.Forms.DeleteOneAsync(x => x.Id == id, cancellationToken).ContinueWith(x => x.Result.DeletedCount > 0, cancellationToken);

    public async Task SetLatestAsync(string department, string category, string currentId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<FormDocument>.Filter.And(
            Builders<FormDocument>.Filter.Eq(x => x.Department, department),
            Builders<FormDocument>.Filter.Eq(x => x.Category, category),
            Builders<FormDocument>.Filter.Ne(x => x.Id, currentId));

        var update = Builders<FormDocument>.Update.Set(x => x.IsLatest, false);
        await context.Forms.UpdateManyAsync(filter, update, cancellationToken: cancellationToken);
    }

    public async Task<FormBrowseFacetsDto> GetBrowseFacetsAsync(CancellationToken cancellationToken = default)
    {
        var countries = await DistinctAsync(nameof(FormDocument.Country), cancellationToken);
        var states = await DistinctAsync(nameof(FormDocument.State), cancellationToken);
        var departments = await DistinctAsync(nameof(FormDocument.Department), cancellationToken);
        var categories = await DistinctAsync(nameof(FormDocument.Category), cancellationToken);
        var languages = await DistinctAsync(nameof(FormDocument.Language), cancellationToken);

        return new FormBrowseFacetsDto(countries, states, departments, categories, languages);
    }

    private async Task<IReadOnlyList<string>> DistinctAsync(string fieldName, CancellationToken cancellationToken)
    {
        var cursor = await context.Forms.DistinctAsync<string>(fieldName, Builders<FormDocument>.Filter.Empty, cancellationToken: cancellationToken);
        return (await cursor.ToListAsync(cancellationToken))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value)
            .ToList();
    }

    private static FilterDefinition<FormDocument> BuildFilter(string query, string? country, string? state, string? department, string? category)
    {
        var filters = new List<FilterDefinition<FormDocument>>();
        if (!string.IsNullOrWhiteSpace(query))
        {
            filters.Add(Builders<FormDocument>.Filter.Text(query));
        }
        if (!string.IsNullOrWhiteSpace(country)) filters.Add(Builders<FormDocument>.Filter.Eq(x => x.Country, country));
        if (!string.IsNullOrWhiteSpace(state)) filters.Add(Builders<FormDocument>.Filter.Eq(x => x.State, state));
        if (!string.IsNullOrWhiteSpace(department)) filters.Add(Builders<FormDocument>.Filter.Eq(x => x.Department, department));
        if (!string.IsNullOrWhiteSpace(category)) filters.Add(Builders<FormDocument>.Filter.Eq(x => x.Category, category));
        return filters.Count == 0 ? Builders<FormDocument>.Filter.Empty : Builders<FormDocument>.Filter.And(filters);
    }
}
