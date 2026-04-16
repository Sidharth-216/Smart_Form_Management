using System.Net.Http.Json;
using Application.Contracts;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public sealed class AiClient(HttpClient httpClient, IOptions<AiSettings> options) : IAiClient
{
    private readonly AiSettings _settings = options.Value;

    public async Task<string> ExtractTextAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"{_settings.BaseUrl}/extract-text", new { file_url = fileUrl }, cancellationToken);
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(cancellationToken: cancellationToken);
        return payload?.GetValueOrDefault("text") ?? string.Empty;
    }

    public async Task<object> ClassifyAsync(string text, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"{_settings.BaseUrl}/classify", new { text }, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken) ?? new { };
    }

    public async Task<IReadOnlyList<string>> SuggestTagsAsync(string text, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"{_settings.BaseUrl}/suggest-tags", new { text }, cancellationToken);
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>(cancellationToken: cancellationToken);
        return payload?.GetValueOrDefault("tags") ?? [];
    }
}
