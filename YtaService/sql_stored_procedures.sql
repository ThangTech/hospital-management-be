-- =====================================================
-- STORED PROCEDURES CHO NHẬP VIỆN VÀ XUẤT VIỆN
-- Chạy script này trong SQL Server Management Studio
-- =====================================================

-- 1. TÌM KIẾM NHẬP VIỆN (cho NhapVien Search)
CREATE OR ALTER PROCEDURE sp_NhapVien_TimKiem
    @TenBenhNhan NVARCHAR(255) = NULL,
    @KhoaId UNIQUEIDENTIFIER = NULL,
    @TrangThai NVARCHAR(50) = NULL,
    @TuNgay DATETIME = NULL,
    @DenNgay DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        nv.Id,
        nv.BenhNhanId,
        bn.HoTen AS TenBenhNhan,
        nv.GiuongId,
        gb.TenGiuong,
        nv.KhoaId,
        kp.TenKhoa,
        nv.LyDoNhap,
        nv.NgayNhap,
        nv.NgayXuat,
        nv.TrangThai
    FROM NhapVien nv
    LEFT JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    LEFT JOIN GiuongBenh gb ON nv.GiuongId = gb.Id
    LEFT JOIN KhoaPhong kp ON nv.KhoaId = kp.Id
    WHERE 
        (@TenBenhNhan IS NULL OR bn.HoTen LIKE '%' + @TenBenhNhan + '%')
        AND (@KhoaId IS NULL OR nv.KhoaId = @KhoaId)
        AND (@TrangThai IS NULL OR nv.TrangThai = @TrangThai)
        AND (@TuNgay IS NULL OR nv.NgayNhap >= @TuNgay)
        AND (@DenNgay IS NULL OR nv.NgayNhap <= @DenNgay)
    ORDER BY nv.NgayNhap DESC
END
GO

-- 2. LẤY DANH SÁCH BỆNH NHÂN SẴN SÀNG XUẤT VIỆN (đã thanh toán hết)
CREATE OR ALTER PROCEDURE sp_XuatVien_LayDanhSachSanSang
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        nv.Id AS NhapVienId,
        bn.HoTen AS TenBenhNhan,
        gb.TenGiuong,
        kp.TenKhoa,
        nv.NgayNhap,
        DATEDIFF(DAY, nv.NgayNhap, GETDATE()) AS SoNgayNam,
        ISNULL(SUM(hd.TongTien), 0) AS TongTien
    FROM NhapVien nv
    LEFT JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    LEFT JOIN GiuongBenh gb ON nv.GiuongId = gb.Id
    LEFT JOIN KhoaPhong kp ON nv.KhoaId = kp.Id
    LEFT JOIN HoaDon hd ON nv.Id = hd.NhapVienId
    WHERE nv.TrangThai = N'Đang điều trị'
      AND NOT EXISTS (
          SELECT 1 FROM HoaDon 
          WHERE NhapVienId = nv.Id AND TrangThai != N'Đã thanh toán'
      )
    GROUP BY nv.Id, bn.HoTen, gb.TenGiuong, kp.TenKhoa, nv.NgayNhap
    ORDER BY nv.NgayNhap
END
GO

-- 3. LẤY LỊCH SỬ XUẤT VIỆN
CREATE OR ALTER PROCEDURE sp_XuatVien_LayLichSu
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        nv.Id AS NhapVienId,
        bn.HoTen AS TenBenhNhan,
        kp.TenKhoa,
        nv.NgayNhap,
        nv.NgayXuat,
        DATEDIFF(DAY, nv.NgayNhap, nv.NgayXuat) AS SoNgayNam,
        nv.LyDoNhap AS GhiChu
    FROM NhapVien nv
    LEFT JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    LEFT JOIN KhoaPhong kp ON nv.KhoaId = kp.Id
    WHERE nv.TrangThai = N'Đã xuất viện'
    ORDER BY nv.NgayXuat DESC
END
GO

-- 4. XEM TRƯỚC THÔNG TIN XUẤT VIỆN
CREATE OR ALTER PROCEDURE sp_XuatVien_XemTruoc
    @NhapVienId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Thông tin chính
    SELECT 
        nv.Id AS NhapVienId,
        bn.HoTen AS TenBenhNhan,
        nv.NgayNhap,
        DATEDIFF(DAY, nv.NgayNhap, GETDATE()) AS SoNgayNam,
        ISNULL(SUM(hd.TongTien), 0) AS TongTienHoaDon,
        ISNULL(SUM(CASE WHEN hd.TrangThai = N'Đã thanh toán' THEN hd.TongTien ELSE 0 END), 0) AS DaThanhToan,
        ISNULL(SUM(CASE WHEN hd.TrangThai != N'Đã thanh toán' THEN hd.TongTien ELSE 0 END), 0) AS ConNo,
        CASE WHEN NOT EXISTS (
            SELECT 1 FROM HoaDon WHERE NhapVienId = nv.Id AND TrangThai != N'Đã thanh toán'
        ) THEN 1 ELSE 0 END AS SanSangXuatVien
    FROM NhapVien nv
    LEFT JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    LEFT JOIN HoaDon hd ON nv.Id = hd.NhapVienId
    WHERE nv.Id = @NhapVienId
    GROUP BY nv.Id, bn.HoTen, nv.NgayNhap
    
    -- Danh sách hóa đơn
    SELECT 
        Id, TongTien, TrangThai, Ngay
    FROM HoaDon
    WHERE NhapVienId = @NhapVienId
    ORDER BY Ngay DESC
END
GO

PRINT N'Tất cả stored procedures đã được tạo thành công!'
