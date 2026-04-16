namespace Domain.Entities;

public sealed class UploadRecord
{
    public string Id { get; set; } = string.Empty;
    public string FormId { get; set; } = string.Empty;
    public string UploadedBy { get; set; } = string.Empty;
    public UploadStatus Status { get; set; } = UploadStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
