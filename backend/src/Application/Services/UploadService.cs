using Application.Contracts;
using Application.DTOs;
using Domain.Entities;

namespace Application.Services;

public sealed class UploadService(IFormRepository forms, IUploadRepository uploads, IFileStorageService storage)
{
    private const long MaxPdfBytes = 25L * 1024 * 1024;

    public async Task<FormDto> UploadAsync(string uploadedBy, UploadFormRequest request, CancellationToken cancellationToken = default)
    {
        if (request.File is null || request.File.Length == 0)
        {
            throw new InvalidOperationException("A PDF file is required.");
        }

        if (request.File.Length > MaxPdfBytes)
        {
            throw new InvalidOperationException("PDF files must be 25 MB or smaller.");
        }

        var extension = Path.GetExtension(request.File.FileName);
        var contentType = request.File.ContentType?.ToLowerInvariant() ?? string.Empty;
        if (!extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase) && !contentType.Contains("pdf"))
        {
            throw new InvalidOperationException("Only PDF files are allowed.");
        }

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.State) || string.IsNullOrWhiteSpace(request.Department) || string.IsNullOrWhiteSpace(request.Category))
        {
            throw new InvalidOperationException("Title, state, department, and category are required.");
        }

        await using var stream = request.File.OpenReadStream();
        var (fileUrl, previewUrl) = await storage.UploadPdfAsync(stream, request.File.FileName, cancellationToken);
        var form = new FormDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Country = string.IsNullOrWhiteSpace(request.Country) ? "India" : request.Country.Trim(),
            State = request.State.Trim(),
            Department = request.Department.Trim(),
            Category = request.Category.Trim(),
            Keywords = Csv(request.KeywordsCsv),
            FileUrl = fileUrl,
            PreviewUrl = previewUrl,
            Language = Csv(request.LanguageCsv),
            Version = string.IsNullOrWhiteSpace(request.Version) ? "1.0.0" : request.Version.Trim(),
            IsLatest = true,
            UploadedBy = uploadedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await forms.CreateAsync(form, cancellationToken);
        await forms.SetLatestAsync(created.Department, created.Category, created.Id, cancellationToken);
        await uploads.CreateAsync(new UploadRecord { Id = Guid.NewGuid().ToString("N"), FormId = created.Id, UploadedBy = uploadedBy, Status = UploadStatus.Pending }, cancellationToken);
        return new FormDto(created.Id, created.Title, created.Description, created.Country, created.State, created.Department, created.Category, created.Keywords, created.FileUrl, created.PreviewUrl, created.Language, created.Version, created.IsLatest, created.UploadedBy, created.CreatedAt, created.UpdatedAt);
    }

    private static List<string> Csv(string value) => value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
}
