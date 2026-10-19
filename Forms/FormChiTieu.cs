using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyChiTieuApp.Logging;
using QuanLyChiTieuApp.Models;
using QuanLyChiTieuApp.Services;

namespace QuanLyChiTieuApp.Forms;

public partial class FormChiTieu : Form
{
    private readonly ChiTieuService _service;
    private List<ChiTieu> _currentList = new();
    private int? _selectedMaCt = null;

    public FormChiTieu()
    {
        InitializeComponent();
        _service = new ChiTieuService();
    }

    private async void FormChiTieu_Load(object sender, EventArgs e)
    {
        try
        {
            // Thiết lập giá trị mặc định cho control nhập liệu
            dtpNgayChi.MaxDate = DateTime.Today;
            dtpNgayChi.Value = DateTime.Today;
            if (cboLoaiChi.Items.Count > 0) cboLoaiChi.SelectedIndex = 0;

            // Thiết lập giá trị mặc định cho lọc: đầu tháng -> hôm nay
            dtpTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpDenNgay.Value = DateTime.Today;
            if (cboLocLoaiChi.Items.Count > 0) cboLocLoaiChi.SelectedIndex = 0; // "Tất cả"

            // Tự động điều chỉnh trạng thái nút
            CapNhatTrangThaiNut(false);

            // Nạp dữ liệu
            await NapDanhSachAsync();
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError(ex, "Khởi tạo form chi tiêu");
            MessageBox.Show(
                "Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra lại cấu hình chuỗi kết nối trong appsettings.json hoặc dịch vụ SQL Server.",
                "Lỗi kết nối CSDL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private async Task NapDanhSachAsync()
    {
        try
        {
            lblStatus.Text = "Đang tải dữ liệu...";
            _currentList = await _service.GetAllAsync();
            dgvChiTieu.DataSource = null;
            dgvChiTieu.DataSource = _currentList;

            CapNhatTongChi();
            lblStatus.Text = $"Đã tải xong {_currentList.Count} khoản chi.";
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError(ex, "Tải danh sách chi tiêu");
            MessageBox.Show(
                "Đã xảy ra lỗi khi tải dữ liệu từ CSDL. Vui lòng thử lại sau.",
                "Lỗi CSDL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
            lblStatus.Text = "Lỗi khi tải dữ liệu.";
        }
    }

    private void CapNhatTongChi()
    {
        decimal tongTien = _currentList.Sum(c => c.SoTien);
        lblTongChi.Text = $"Tổng chi: {tongTien:N0} đ";
    }

    private void dgvChiTieu_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= dgvChiTieu.Rows.Count) return;

        var colName = dgvChiTieu.Columns[e.ColumnIndex].Name;

        // Định dạng Mã hiển thị 3 chữ số: 001, 002...
        if (colName == "colMaCT" && e.Value != null)
        {
            if (int.TryParse(e.Value.ToString(), out int ma))
            {
                e.Value = ma.ToString("D3");
                e.FormattingApplied = true;
            }
        }
        // Định dạng Ngày chi: dd/MM/yyyy
        else if (colName == "colNgayChi" && e.Value != null)
        {
            if (e.Value is DateOnly d)
            {
                e.Value = d.ToString("dd/MM/yyyy");
                e.FormattingApplied = true;
            }
        }
        // Định dạng Số tiền có phân cách hàng nghìn
        else if (colName == "colSoTien" && e.Value != null)
        {
            if (decimal.TryParse(e.Value.ToString(), out decimal st))
            {
                e.Value = $"{st:N0}";
                e.FormattingApplied = true;
            }
        }
    }

    private void dgvChiTieu_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= dgvChiTieu.Rows.Count) return;

        var selected = dgvChiTieu.Rows[e.RowIndex].DataBoundItem as ChiTieu;
        if (selected == null) return;

        _selectedMaCt = selected.MaCt;
        dtpNgayChi.Value = selected.NgayChi.ToDateTime(TimeOnly.MinValue);
        txtNoiDung.Text = selected.NoiDung;
        cboLoaiChi.SelectedItem = selected.LoaiChi;
        nudSoTien.Value = selected.SoTien;
        txtGhiChu.Text = selected.GhiChu ?? string.Empty;

        CapNhatTrangThaiNut(true);
        lblStatus.Text = $"Đang chọn khoản chi mã: {_selectedMaCt:D3} - {selected.NoiDung}";
    }

    private bool KiemTraHopLe()
    {
        // 1. Kiểm tra nội dung không rỗng
        if (string.IsNullOrWhiteSpace(txtNoiDung.Text))
        {
            MessageBox.Show(
                "Nội dung chi tiêu không được để trống!",
                "Cảnh báo hợp lệ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            txtNoiDung.Focus();
            return false;
        }

        // 2. Kiểm tra số tiền > 0
        if (nudSoTien.Value <= 0)
        {
            MessageBox.Show(
                "Số tiền chi tiêu phải lớn hơn 0 đ!",
                "Cảnh báo hợp lệ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            nudSoTien.Focus();
            return false;
        }

        // 3. Kiểm tra ngày chi không lớn hơn hôm nay
        if (dtpNgayChi.Value.Date > DateTime.Today)
        {
            MessageBox.Show(
                "Ngày chi tiêu không được lớn hơn ngày hôm nay!",
                "Cảnh báo hợp lệ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            dtpNgayChi.Focus();
            return false;
        }

        return true;
    }

    private async void btnThem_Click(object sender, EventArgs e)
    {
        if (!KiemTraHopLe()) return;

        try
        {
            var chiTieu = new ChiTieu
            {
                NgayChi = DateOnly.FromDateTime(dtpNgayChi.Value),
                NoiDung = txtNoiDung.Text.Trim(),
                LoaiChi = cboLoaiChi.SelectedItem?.ToString() ?? "Khác",
                SoTien = nudSoTien.Value,
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim()
            };

            await _service.AddAsync(chiTieu);

            // Ghi log mức Information
            LoggingHelper.LogAdd(chiTieu.MaCt, chiTieu.NoiDung, chiTieu.SoTien, chiTieu.LoaiChi);

            MessageBox.Show(
                $"Đã thêm thành công khoản chi '{chiTieu.NoiDung}' (Mã: {chiTieu.MaCt:D3})!",
                "Thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            await NapDanhSachAsync();
            LamMoiForm();
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError(ex, "Thêm khoản chi tiêu mới");
            MessageBox.Show(
                "Đã xảy ra lỗi khi thêm dữ liệu vào cơ sở dữ liệu. Vui lòng kiểm tra lại kết nối.",
                "Lỗi CSDL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private async void btnSua_Click(object sender, EventArgs e)
    {
        if (!_selectedMaCt.HasValue)
        {
            MessageBox.Show(
                "Vui lòng chọn một dòng khoản chi cần sửa từ danh sách!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            return;
        }

        if (!KiemTraHopLe()) return;

        try
        {
            var chiTieu = new ChiTieu
            {
                MaCt = _selectedMaCt.Value,
                NgayChi = DateOnly.FromDateTime(dtpNgayChi.Value),
                NoiDung = txtNoiDung.Text.Trim(),
                LoaiChi = cboLoaiChi.SelectedItem?.ToString() ?? "Khác",
                SoTien = nudSoTien.Value,
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim()
            };

            bool success = await _service.UpdateAsync(chiTieu);
            if (success)
            {
                // Ghi log mức Information
                LoggingHelper.LogUpdate(chiTieu.MaCt, chiTieu.NoiDung, chiTieu.SoTien, chiTieu.LoaiChi);

                MessageBox.Show(
                    $"Đã cập nhật thành công khoản chi '{chiTieu.NoiDung}' (Mã: {chiTieu.MaCt:D3})!",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                await NapDanhSachAsync();
                LamMoiForm();
            }
            else
            {
                MessageBox.Show("Không tìm thấy khoản chi cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError(ex, $"Cập nhật khoản chi MaCT: {_selectedMaCt}");
            MessageBox.Show(
                "Đã xảy ra lỗi khi cập nhật dữ liệu. Vui lòng kiểm tra lại kết nối.",
                "Lỗi CSDL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private async void btnXoa_Click(object sender, EventArgs e)
    {
        if (!_selectedMaCt.HasValue)
        {
            MessageBox.Show(
                "Vui lòng chọn một dòng khoản chi cần xóa từ danh sách!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            return;
        }

        // Hỏi xác nhận trước khi xóa
        var dialogResult = MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa khoản chi '{txtNoiDung.Text}' (Mã: {_selectedMaCt:D3}) không?\nThao tác này không thể hoàn tác!",
            "Xác nhận xóa",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (dialogResult != DialogResult.Yes) return;

        try
        {
            int maCt = _selectedMaCt.Value;
            string noiDung = txtNoiDung.Text;
            bool success = await _service.DeleteAsync(maCt);

            if (success)
            {
                // Ghi log mức Information
                LoggingHelper.LogDelete(maCt, noiDung);

                MessageBox.Show(
                    $"Đã xóa thành công khoản chi (Mã: {maCt:D3})!",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                await NapDanhSachAsync();
                LamMoiForm();
            }
            else
            {
                MessageBox.Show("Không tìm thấy khoản chi cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError(ex, $"Xóa khoản chi MaCT: {_selectedMaCt}");
            MessageBox.Show(
                "Đã xảy ra lỗi khi xóa khoản chi. Vui lòng kiểm tra lại kết nối.",
                "Lỗi CSDL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void btnLamMoi_Click(object sender, EventArgs e)
    {
        LamMoiForm();
        lblStatus.Text = "Đã làm mới thông tin nhập liệu.";
    }

    private void LamMoiForm()
    {
        _selectedMaCt = null;
        dtpNgayChi.Value = DateTime.Today;
        txtNoiDung.Clear();
        if (cboLoaiChi.Items.Count > 0) cboLoaiChi.SelectedIndex = 0;
        nudSoTien.Value = 0;
        txtGhiChu.Clear();
        dgvChiTieu.ClearSelection();

        CapNhatTrangThaiNut(false);
    }

    private void CapNhatTrangThaiNut(bool dangChonDong)
    {
        btnSua.Enabled = dangChonDong;
        btnXoa.Enabled = dangChonDong;
        btnThem.Enabled = !dangChonDong;
    }

    private async void btnTim_Click(object sender, EventArgs e)
    {
        await ThucHienLocAsync();
    }

    private async Task ThucHienLocAsync()
    {
        try
        {
            string keyword = txtTimNoiDung.Text.Trim();
            string loaiChi = cboLocLoaiChi.SelectedItem?.ToString() ?? "Tất cả";
            DateOnly tuNgay = DateOnly.FromDateTime(dtpTuNgay.Value.Date);
            DateOnly denNgay = DateOnly.FromDateTime(dtpDenNgay.Value.Date);

            if (tuNgay > denNgay)
            {
                MessageBox.Show(
                    "Khoảng thời gian không hợp lệ: 'Từ ngày' phải nhỏ hơn hoặc bằng 'Đến ngày'!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            lblStatus.Text = "Đang lọc dữ liệu...";
            _currentList = await _service.FilterAsync(keyword, loaiChi, tuNgay, denNgay);

            dgvChiTieu.DataSource = null;
            dgvChiTieu.DataSource = _currentList;

            CapNhatTongChi();
            lblStatus.Text = $"Kết quả lọc: Tìm thấy {_currentList.Count} khoản chi phù hợp.";
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError(ex, "Thực hiện tìm kiếm/lọc khoản chi");
            MessageBox.Show(
                "Đã xảy ra lỗi khi lọc dữ liệu. Vui lòng kiểm tra lại kết nối.",
                "Lỗi CSDL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private async void btnBoLoc_Click(object sender, EventArgs e)
    {
        txtTimNoiDung.Clear();
        if (cboLocLoaiChi.Items.Count > 0) cboLocLoaiChi.SelectedIndex = 0; // "Tất cả"
        dtpTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        dtpDenNgay.Value = DateTime.Today;

        await NapDanhSachAsync();
        lblStatus.Text = "Đã bỏ bộ lọc. Hiển thị tất cả khoản chi.";
    }

    private async void txtTimNoiDung_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            await ThucHienLocAsync();
        }
    }
}
