namespace Infrastructure.Settings;

public sealed class RedisSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public bool Enabled => !string.IsNullOrWhiteSpace(ConnectionString);
}
