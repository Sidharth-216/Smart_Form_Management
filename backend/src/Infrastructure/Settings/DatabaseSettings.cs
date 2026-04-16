namespace Infrastructure.Settings;

public sealed class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "form_management";
}
