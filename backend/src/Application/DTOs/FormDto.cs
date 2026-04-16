namespace Application.DTOs;

public sealed record FormDto(
    string Id,
    string Title,
    string Description,
    string Country,
    string State,
    string Department,
    string Category,
    IReadOnlyList<string> Keywords,
    string FileUrl,
    string PreviewUrl,
    IReadOnlyList<string> Language,
    string Version,
    bool IsLatest,
    string UploadedBy,
    DateTime CreatedAt,
    DateTime UpdatedAt);
