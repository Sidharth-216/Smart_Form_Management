using Application.Contracts;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public sealed class FileStorageService(HttpClient httpClient, IOptions<StorageSettings> options) : IFileStorageService
{
    private readonly StorageSettings _settings = options.Value;

    public async Task<(string FileUrl, string PreviewUrl)> UploadPdfAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(_settings.UploadEndpoint))
        {
            using var content = new StreamContent(fileStream);
            using var request = new HttpRequestMessage(HttpMethod.Post, _settings.UploadEndpoint)
            {
                Content = content
            };
            request.Headers.Add("X-File-Name", fileName);
            var response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var remote = await response.Content.ReadAsStringAsync(cancellationToken);
            return (remote, remote);
        }

        var safeName = Path.GetFileNameWithoutExtension(fileName);
        var slug = string.Concat(safeName.Where(char.IsLetterOrDigit)).ToLowerInvariant();
        var fileUrl = $"{_settings.PublicBaseUrl.TrimEnd('/')}/forms/{slug}.pdf";
        var previewUrl = $"{_settings.PublicBaseUrl.TrimEnd('/')}/forms/{slug}-preview.pdf";
        return (fileUrl, previewUrl);
    }
}
