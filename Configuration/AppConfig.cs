namespace QuanLyChiTieuApp.Configuration;

public class AppConfig
{
    public ConnectionStringsConfig ConnectionStrings { get; set; } = new();
    public ApplicationSettingsConfig ApplicationSettings { get; set; } = new();
}

public class ConnectionStringsConfig
{
    public string DefaultConnection { get; set; } = string.Empty;
}

public class ApplicationSettingsConfig
{
    public string AppName { get; set; } = "Sổ chi tiêu cá nhân";
    public string Version { get; set; } = "1.0.0";
}
