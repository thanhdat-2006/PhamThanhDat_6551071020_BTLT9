using System;
using System.IO;
using Serilog;

namespace QuanLyChiTieuApp.Logging;

public static class LoggingHelper
{
    private static bool _initialized = false;

    public static void Initialize()
    {
        if (_initialized) return;

        var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }

        // Cấu hình file log theo ngày: Logs/log-yyyyMMdd.txt
        var logFilePath = Path.Combine(logDir, "log-.txt");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 31
            )
            .CreateLogger();

        _initialized = true;
        Log.Information("=== Ứng dụng Sổ chi tiêu cá nhân đã khởi động ===");
    }

    public static void LogInfo(string message)
    {
        Initialize();
        Log.Information(message);
    }

    public static void LogAdd(int maCt, string noiDung, decimal soTien, string loaiChi)
    {
        Initialize();
        Log.Information("Thêm khoản chi thành công [MaCT: {MaCT}] - Nội dung: {NoiDung}, Số tiền: {SoTien:N0} đ, Loại chi: {LoaiChi}",
            maCt, noiDung, soTien, loaiChi);
    }

    public static void LogUpdate(int maCt, string noiDung, decimal soTien, string loaiChi)
    {
        Initialize();
        Log.Information("Cập nhật khoản chi thành công [MaCT: {MaCT}] - Nội dung: {NoiDung}, Số tiền: {SoTien:N0} đ, Loại chi: {LoaiChi}",
            maCt, noiDung, soTien, loaiChi);
    }

    public static void LogDelete(int maCt, string noiDung)
    {
        Initialize();
        Log.Information("Xóa khoản chi thành công [MaCT: {MaCT}] - Nội dung: {NoiDung}",
            maCt, noiDung);
    }

    public static void LogError(Exception ex, string contextMessage)
    {
        Initialize();
        Log.Error(ex, "Lỗi truy cập CSDL hoặc hệ thống: {ContextMessage}", contextMessage);
    }

    public static void CloseAndFlush()
    {
        Log.CloseAndFlush();
    }
}
