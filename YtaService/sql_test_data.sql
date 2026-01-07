-- =====================================================
-- SQL TEST DATA: Hỗ trợ kiểm tra API Xuất viện
-- Chạy script này để tạo dữ liệu mẫu cho danh sách "Sẵn sàng xuất viện"
-- =====================================================

-- 1. Tìm hoặc tạo Bệnh nhân có BHYT (80%)
DECLARE @MyBenhNhanId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM BenhNhan WHERE SoTheBaoHiem IS NOT NULL);
IF @MyBenhNhanId IS NULL
BEGIN
    SET @MyBenhNhanId = NEWID();
    INSERT INTO BenhNhan (Id, HoTen, SoTheBaoHiem, MucHuong)
    VALUES (@MyBenhNhanId, N'Nguyễn Văn Bảo Hiểm', 'BHYT123456789', 0.8);
END
ELSE
BEGIN
    UPDATE BenhNhan SET MucHuong = 0.8 WHERE Id = @MyBenhNhanId;
END

DECLARE @MyKhoaId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM KhoaPhong);
DECLARE @MyGiuongId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM GiuongBenh WHERE TrangThai = N'Trống');

IF @MyGiuongId IS NULL
BEGIN
    PRINT N'LỖI: Cần 1 Giường trống để chạy test.';
END
ELSE
BEGIN
    DECLARE @NewNhapVienId UNIQUEIDENTIFIER = NEWID();

    -- Bước 1: Tạo phiếu Nhập viện
    INSERT INTO NhapVien (Id, BenhNhanId, GiuongId, KhoaId, LyDoNhap, NgayNhap, TrangThai)
    VALUES (@NewNhapVienId, @MyBenhNhanId, @MyGiuongId, @MyKhoaId, N'Test BHYT', GETDATE(), N'Đang điều trị');

    -- Bước 2: Cập nhật giường
    UPDATE GiuongBenh SET TrangThai = N'Có người' WHERE Id = @MyGiuongId;

    -- Bước 3: Tạo 1 hóa đơn và ĐÃ THANH TOÁN (để đủ điều kiện xuất hiện trong LayDanhSach)
    DECLARE @HoaDonId UNIQUEIDENTIFIER = NEWID();
    INSERT INTO HoaDon (Id, BenhNhanId, NhapVienId, TongTien, BaoHiemChiTra, BenhNhanThanhToan, Ngay, TrangThai)
    VALUES (@HoaDonId, @MyBenhNhanId, @NewNhapVienId, 1000000, 800000, 200000, GETDATE(), N'Đã thanh toán');

    PRINT N'=== DỮ LIỆU ĐÃ SẴN SÀNG XUẤT VIỆN ===';
    PRINT N'1. BenhNhanId: ' + CAST(@MyBenhNhanId AS NVARCHAR(50));
    PRINT N'2. NhapVienId: ' + CAST(@NewNhapVienId AS NVARCHAR(50));
    PRINT N'3. Tình trạng: Đã đóng đủ tiền, đang nằm viện.';
    PRINT N'======================================';
    PRINT N'HƯỚNG DẪN TEST:';
    PRINT N'- Gọi GET /api/XuatVien/LayDanhSach -> Bệnh nhân này sẽ xuất hiện!';
    PRINT N'======================================';
END
GO
