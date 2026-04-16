namespace Application.Contracts;

public interface IFileStorageService
{
    Task<(string FileUrl, string PreviewUrl)> UploadPdfAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}
