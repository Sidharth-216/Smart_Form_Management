namespace Application.Contracts;

public interface IAiClient
{
    Task<string> ExtractTextAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<object> ClassifyAsync(string text, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> SuggestTagsAsync(string text, CancellationToken cancellationToken = default);
}
