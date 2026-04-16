namespace Infrastructure.Settings;

public sealed class StorageSettings
{
    public string Provider { get; set; } = "local";
    public string PublicBaseUrl { get; set; } = string.Empty;
    public string UploadEndpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string Bucket { get; set; } = string.Empty;
}
