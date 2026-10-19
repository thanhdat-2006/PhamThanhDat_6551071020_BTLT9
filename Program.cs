using System;
using System.Windows.Forms;
using QuanLyChiTieuApp.Configuration;
using QuanLyChiTieuApp.Forms;
using QuanLyChiTieuApp.Logging;

namespace QuanLyChiTieuApp;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Khởi tạo cấu hình và Serilog
        try
        {
            ConfigurationHelper.LoadConfiguration();
            LoggingHelper.Initialize();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Lỗi khởi tạo cấu hình hoặc nhật ký: {ex.Message}",
                "Lỗi khởi động",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        try
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FormChiTieu());
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError(ex, "Lỗi ứng dụng không xử lý được (Unhandled Exception)");
            MessageBox.Show(
                $"Đã xảy ra lỗi nghiêm trọng: {ex.Message}",
                "Lỗi hệ thống",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        finally
        {
            LoggingHelper.CloseAndFlush();
        }
    }    
}