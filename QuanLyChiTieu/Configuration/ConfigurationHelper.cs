using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace QuanLyChiTieuApp.Configuration;

public static class ConfigurationHelper
{
    private static AppConfig? _appConfig;
    private static IConfigurationRoot? _configuration;

    public static AppConfig Config
    {
        get
        {
            if (_appConfig == null)
            {
                LoadConfiguration();
            }
            return _appConfig!;
        }
    }

    public static void LoadConfiguration()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

        _configuration = builder.Build();

        _appConfig = new AppConfig();
        _configuration.Bind(_appConfig);

        // Đảm bảo lấy được chuỗi kết nối
        if (string.IsNullOrWhiteSpace(_appConfig.ConnectionStrings?.DefaultConnection))
        {
            var connStr = _configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                _appConfig.ConnectionStrings ??= new ConnectionStringsConfig();
                _appConfig.ConnectionStrings.DefaultConnection = connStr;
            }
        }
    }

    public static string GetConnectionString()
    {
        return Config.ConnectionStrings.DefaultConnection;
    }
}
