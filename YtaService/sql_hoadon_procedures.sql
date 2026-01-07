-- =====================================================
-- STORED PROCEDURES CHO MODULE HÓA ĐƠN (HoaDon)
-- Chạy script này trong SQL Server Management Studio
-- =====================================================

-- 1. TẠO HÓA ĐƠN MỚI
CREATE OR ALTER PROCEDURE sp_HoaDon_Create
    @Id UNIQUEIDENTIFIER,
    @BenhNhanId UNIQUEIDENTIFIER,
    @NhapVienId UNIQUEIDENTIFIER,
    @TongTien DECIMAL(18,2),
    @BaoHiemChiTra DECIMAL(18,2) = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Kiểm tra nhập viện có tồn tại không
    IF NOT EXISTS (SELECT 1 FROM NhapVien WHERE Id = @NhapVienId)
    BEGIN
        RETURN 0 -- Không tìm thấy phiếu nhập viện
    END

    INSERT INTO HoaDon (Id, BenhNhanId, NhapVienId, TongTien, BaoHiemChiTra, BenhNhanThanhToan, Ngay, TrangThai)
    VALUES (@Id, @BenhNhanId, @NhapVienId, @TongTien, @BaoHiemChiTra, 0, GETDATE(), N'Chưa thanh toán')
    
    RETURN 1 -- Thành công
END
GO

-- 2. LẤY DANH SÁCH HÓA ĐƠN (Có filter)
CREATE OR ALTER PROCEDURE sp_HoaDon_GetList
    @BenhNhanId UNIQUEIDENTIFIER = NULL,
    @NhapVienId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        hd.*,
        bn.HoTen
    FROM HoaDon hd
    JOIN BenhNhan bn ON hd.BenhNhanId = bn.Id
    WHERE (@BenhNhanId IS NULL OR hd.BenhNhanId = @BenhNhanId)
      AND (@NhapVienId IS NULL OR hd.NhapVienId = @NhapVienId)
    ORDER BY hd.Ngay DESC
END
GO

-- 3. LẤY CHI TIẾT HÓA ĐƠN THEO ID
CREATE OR ALTER PROCEDURE sp_HoaDon_GetById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        hd.*,
        bn.HoTen
    FROM HoaDon hd
    JOIN BenhNhan bn ON hd.BenhNhanId = bn.Id
    WHERE hd.Id = @Id
END
GO

-- 4. THANH TOÁN HÓA ĐƠN
CREATE OR ALTER PROCEDURE sp_HoaDon_ThanhToan
    @Id UNIQUEIDENTIFIER,
    @SoTien DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Kiểm tra hóa đơn tồn tại
    DECLARE @TongTien DECIMAL(18,2), @BaoHiem DECIMAL(18,2), @DaThanhToan DECIMAL(18,2)
    SELECT @TongTien = TongTien, @BaoHiem = BaoHiemChiTra, @DaThanhToan = BenhNhanThanhToan
    FROM HoaDon WHERE Id = @Id
    
    IF @TongTien IS NULL RETURN 0

    DECLARE @CanThanhToan DECIMAL(18,2) = @TongTien - @BaoHiem
    
    -- Cập nhật số tiền bệnh nhân đã trả
    UPDATE HoaDon
    SET BenhNhanThanhToan = BenhNhanThanhToan + @SoTien
    WHERE Id = @Id
    
    -- Nếu đã trả đủ (hoặc thừa) thì chuyển sang Đã thanh toán
    IF (SELECT BenhNhanThanhToan FROM HoaDon WHERE Id = @Id) >= @CanThanhToan
    BEGIN
        UPDATE HoaDon SET TrangThai = N'Đã thanh toán' WHERE Id = @Id
    END
    
    RETURN 1
END
GO

-- 5. XÓA HÓA ĐƠN
CREATE OR ALTER PROCEDURE sp_HoaDon_Delete
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Chỉ cho xóa hóa đơn chưa thanh toán hoặc hóa đơn nhầm
    IF EXISTS (SELECT 1 FROM HoaDon WHERE Id = @Id AND TrangThai = N'Đã thanh toán')
    BEGIN
        RETURN 0 -- Không cho xóa hóa đơn đã thanh toán
    END

    DELETE FROM HoaDon WHERE Id = @Id
    RETURN 1
END
GO

PRINT N'Tất cả stored procedures cho Hóa đơn đã được tạo thành công!'
