# Ứng Dụng WinForms: Sổ Chi Tiêu Cá Nhân (EF Core Database First)

> **Môn học:** Lập trình ứng dụng C# / Windows Forms  
> **Chương 11 - Bài tập 1:** Xây dựng ứng dụng quản lý chi tiêu cá nhân sử dụng EF Core Database First, cấu hình Strongly-typed qua `appsettings.json`, ghi nhật ký với `Serilog`, và đóng gói Framework-Dependent.

---

## 1. Cấu trúc thư mục dự án

```text
QuanLyChiTieuApp/
├── Configuration/
│   ├── AppConfig.cs                 # Lớp cấu hình strongly-typed ánh xạ từ appsettings.json
│   └── ConfigurationHelper.cs       # Đọc cấu hình bằng ConfigurationBuilder
├── Forms/
│   ├── FormChiTieu.cs               # Xử lý sự kiện, CRUD, tìm kiếm, lọc, tính tổng tiền, bắt lỗi
│   └── FormChiTieu.Designer.cs      # Thiết kế giao diện WinForms hiện đại, chuẩn tỉ lệ
├── Logging/
│   └── LoggingHelper.cs             # Cấu hình Serilog ghi file theo ngày vào thư mục Logs
├── Models/
│   ├── ChiTieu.cs                   # Model thực thể bảng ChiTieu (scaffolded từ CSDL)
│   └── QlChiTieuContext.cs          # DbContext EF Core (kết nối CSDL qua cấu hình)
├── Services/
│   └── ChiTieuService.cs            # Tầng dịch vụ xử lý truy vấn và thao tác CSDL
├── publish/                         # Thư mục đóng gói Framework-Dependent win-x64
├── appsettings.json                 # Cấu hình chính thức (chuỗi kết nối CSDL)
├── appsettings.Development.json     # Cấu hình môi trường phát triển (ghi đè khi code)
├── appsettings.example.json         # File cấu hình mẫu để tham khảo
├── .gitignore                       # Bỏ qua appsettings.Development.json, Logs/, bin/, obj/
├── QL_ChiTieu.sql                   # Script SQL tạo CSDL QL_ChiTieu và 12 dòng dữ liệu mẫu
├── Program.cs                       # Điểm khởi chạy ứng dụng, khởi tạo config & logger
└── QuanLyChiTieuApp.csproj          # File cấu hình dự án .NET (Copy if newer cho appsettings)
```

---

## 2. Hướng dẫn cài đặt CSDL SQL Server

1. Mở **SQL Server Management Studio (SSMS)** hoặc **Azure Data Studio**.
2. Mở file [QL_ChiTieu.sql](file:///d:/C%23/QuanLyChiTieuApp/QL_ChiTieu.sql).
3. Bấm **Execute** (hoặc `F5`) để tạo CSDL `QL_ChiTieu`, bảng `ChiTieu` và nạp sẵn 12 dòng dữ liệu mẫu (Ăn uống, Đi lại, Học tập, Giải trí, Khác).

---

## 3. Các tính năng đã hoàn thiện

### Task 1: CSDL và Scaffold
- Tạo bảng `ChiTieu` gồm các cột: `MaCT` (Identity, PK), `NgayChi` (date), `NoiDung` (nvarchar 100), `LoaiChi` (nvarchar 30), `SoTien` (decimal 18,0), `GhiChu` (nvarchar 200, null).
- Scaffold bằng `dotnet ef dbcontext scaffold` sinh `QlChiTieuContext` và `ChiTieu`.

### Task 2 & 3: Giao diện và CRUD
- **DateTimePicker `dtpNgayChi`**: chọn ngày chi (dd/MM/yyyy), mặc định ngày hôm nay.
- **TextBox `txtNoiDung`**: nhập nội dung chi tiêu.
- **ComboBox `cboLoaiChi`**: chọn loại chi (Ăn uống, Đi lại, Học tập, Giải trí, Khác).
- **NumericUpDown `nudSoTien`**: nhập số tiền chi tiêu (bước nhảy 1.000, có phân cách hàng nghìn).
- **TextBox nhiều dòng `txtGhiChu`**: ghi chú chi tiết.
- **DataGridView `dgvChiTieu`**: hiển thị danh sách trực quan, mã định dạng 3 số (001, 002...), số tiền phân cách hàng nghìn.
- **Kiểm tra hợp lệ đầy đủ**:
  - Nội dung không được để trống.
  - Số tiền phải lớn hơn 0.
  - Ngày chi không được lớn hơn ngày hiện tại.
- **Thao tác dữ liệu**:
  - Bấm vào một dòng trong danh sách: tự động nạp dữ liệu lên các ô nhập liệu để sửa.
  - Xóa: có hộp thoại xác nhận trước khi xóa.
  - Nút Làm mới: xóa trắng dữ liệu nhập liệu để nhập khoản chi mới.

### Task 4: Tìm kiếm và Lọc
- Ô tìm kiếm theo nội dung chi tiêu.
- Lọc theo loại chi ("Tất cả", "Ăn uống", "Đi lại", "Học tập", "Giải trí", "Khác").
- Lọc theo khoảng ngày (Từ ngày – Đến ngày).
- **Label "Tổng chi: ... đ"**: tự động cập nhật tổng số tiền theo đúng các dòng đang hiển thị trên lưới.

### Task 5: Cấu hình linh hoạt (9.2)
- Chuỗi kết nối được đưa ra `appsettings.json` và `appsettings.Development.json`.
- Sử dụng `ConfigurationBuilder` và ánh xạ vào lớp cấu hình có kiểu `AppConfig`.
- File `.csproj` cấu hình `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>` cho các file cấu hình.
- Thêm `appsettings.Development.json` vào `.gitignore` và tạo sẵn `appsettings.example.json`.

### Task 6: Ghi nhật ký Serilog (9.3)
- Tự động tạo file log theo ngày vào thư mục `Logs/` cạnh file `.exe` (ví dụ: `Logs/log-20261019.txt`).
- Ghi mức **Information** khi Thêm/Sửa/Xóa thành công (có kèm `MaCT`, nội dung, số tiền).
- Ghi mức **Error** khi có lỗi kết nối CSDL và hiển thị hộp thoại thông báo thân thiện cho người dùng.

### Task 7: Đóng gói và triển khai sang máy khác (9.1)
- Lệnh publish Framework-Dependent:
  ```bash
  dotnet publish -c Release -r win-x64 --no-self-contained -o ./publish
  ```
- Thư mục `./publish` đã có đầy đủ file thực thi `QuanLyChiTieuApp.exe`, file `appsettings.json` và các thư viện cần thiết.
- **Cách chạy trên máy khác**:
  1. Cài đặt **.NET Desktop Runtime** (phiên bản tương ứng).
  2. Chép toàn bộ thư mục `publish` sang máy mới.
  3. Mở file `appsettings.json` trong thư mục `publish`, chỉnh sửa chuỗi kết nối `DefaultConnection` trỏ đúng tên SQL Server của máy đó.
  4. Chạy trực tiếp `QuanLyChiTieuApp.exe` mà **không cần build lại**.
