-- Script tạo CSDL và bảng cho bài toán Quản lý chi tiêu cá nhân (Bài 1)
-- CSDL: QL_ChiTieu
-- Bảng: ChiTieu

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'QL_ChiTieu')
BEGIN
    CREATE DATABASE QL_ChiTieu;
END
GO

USE QL_ChiTieu;
GO

IF OBJECT_ID(N'dbo.ChiTieu', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.ChiTieu;
END
GO

CREATE TABLE dbo.ChiTieu (
    MaCT INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    NgayChi DATE NOT NULL,
    NoiDung NVARCHAR(100) NOT NULL,
    LoaiChi NVARCHAR(30) NOT NULL,
    SoTien DECIMAL(18, 0) NOT NULL,
    GhiChu NVARCHAR(200) NULL
);
GO

-- Chèn ít nhất 10 dòng dữ liệu mẫu
INSERT INTO dbo.ChiTieu (NgayChi, NoiDung, LoaiChi, SoTien, GhiChu) VALUES
('2026-10-01', N'Cơm trưa', N'Ăn uống', 35000, N'Cơm sườn bì chả'),
('2026-10-02', N'Xăng xe', N'Đi lại', 60000, N'Đổ xăng xe máy'),
('2026-10-03', N'Photo giáo trình', N'Học tập', 25000, N'Tài liệu môn C# WinForms'),
('2026-10-04', N'Xem phim', N'Giải trí', 90000, N'Vé xem phim cuối tuần'),
('2026-10-05', N'Trà sữa', N'Ăn uống', 40000, N'Trà sữa trân châu'),
('2026-10-06', N'Mua sắm', N'Khác', 150000, N'Dụng cụ sinh hoạt cá nhân'),
('2026-10-07', N'Tiền điện', N'Khác', 500000, N'Thanh toán hóa đơn tiền điện'),
('2026-10-08', N'Ăn sáng', N'Ăn uống', 25000, N'Bánh mì xíu mại'),
('2026-10-09', N'Mua sách C#', N'Học tập', 120000, N'Giáo trình lập trình .NET Core'),
('2026-10-10', N'Vé xe buýt tháng', N'Đi lại', 100000, N'Vé xe buýt sinh viên'),
('2026-10-12', N'Cà phê với bạn', N'Ăn uống', 45000, N'Gặp mặt bạn cấp 3'),
('2026-10-15', N'Bida giải trí', N'Giải trí', 80000, N'Chơi bida với bạn cùng phòng');
GO

-- Kiểm tra dữ liệu vừa chèn
SELECT * FROM dbo.ChiTieu;
GO
