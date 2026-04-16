namespace Domain.Entities;

public sealed class FormDocument
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Country { get; set; } = "India";
    public string State { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = [];
    public string FileUrl { get; set; } = string.Empty;
    public string PreviewUrl { get; set; } = string.Empty;
    public List<string> Language { get; set; } = [];
    public string Version { get; set; } = "1.0.0";
    public bool IsLatest { get; set; } = true;
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
