-- =====================================================
-- STORED PROCEDURES CHO MODULE HÓA ĐƠN (HoaDon) - CẬP NHẬT
-- =====================================================

-- 1. TẠO HÓA ĐƠN MỚI (Tự động tính BHYT)
CREATE OR ALTER PROCEDURE sp_HoaDon_Create
    @Id UNIQUEIDENTIFIER,
    @BenhNhanId UNIQUEIDENTIFIER,
    @NhapVienId UNIQUEIDENTIFIER,
    @TongTien DECIMAL(18,2),
    @BaoHiemChiTra_Manual DECIMAL(18,2) = NULL -- Nếu truyền vào thì lấy giá trị này, nếu không thì tự tính
AS
BEGIN
    SET NOCOUNT ON;
    
    -- 1. Kiểm tra nhập viện có tồn tại không
    IF NOT EXISTS (SELECT 1 FROM NhapVien WHERE Id = @NhapVienId)
    BEGIN
        RETURN 0 -- Không tìm thấy phiếu nhập viện
    END

    -- 2. Lấy mức hưởng bảo hiểm của bệnh nhân
    DECLARE @MucHuong DECIMAL(3,2), @SoTheBHYT NVARCHAR(50)
    SELECT @MucHuong = ISNULL(MucHuong, 0), @SoTheBHYT = SoTheBaoHiem 
    FROM BenhNhan WHERE Id = @BenhNhanId

    -- 3. Logic tính toán BHYT chi trả
    DECLARE @FinBaoHiem DECIMAL(18,2) = @BaoHiemChiTra_Manual
    
    IF @FinBaoHiem IS NULL -- Nếu không nhập thủ công thì tự tính
    BEGIN
        IF @SoTheBHYT IS NOT NULL AND @MucHuong > 0
            SET @FinBaoHiem = @TongTien * @MucHuong
        ELSE
            SET @FinBaoHiem = 0
    END

    -- 4. Chèn hóa đơn
    INSERT INTO HoaDon (Id, BenhNhanId, NhapVienId, TongTien, BaoHiemChiTra, BenhNhanThanhToan, Ngay, TrangThai)
    VALUES (@Id, @BenhNhanId, @NhapVienId, @TongTien, @FinBaoHiem, 0, GETDATE(), N'Chưa thanh toán')
    
    RETURN 1 -- Thành công
END
GO

-- 4. THANH TOÁN HÓA ĐƠN (Cập nhật logic chặn trả thừa)
CREATE OR ALTER PROCEDURE sp_HoaDon_ThanhToan
    @Id UNIQUEIDENTIFIER,
    @SoTien DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TongTien DECIMAL(18,2), @BaoHiem DECIMAL(18,2), @DaThanhToan DECIMAL(18,2), @TrangThai NVARCHAR(50)
    SELECT @TongTien = TongTien, @BaoHiem = BaoHiemChiTra, @DaThanhToan = BenhNhanThanhToan, @TrangThai = TrangThai
    FROM HoaDon WHERE Id = @Id
    
    IF @TongTien IS NULL RETURN 0 -- Không tìm thấy hóa đơn
    
    IF @TrangThai = N'Đã thanh toán' RETURN -1 -- Hóa đơn đã thanh toán xong rồi

    DECLARE @CanThanhToan DECIMAL(18,2) = @TongTien - @BaoHiem
    DECLARE @ConLai DECIMAL(18,2) = @CanThanhToan - @DaThanhToan
    
    -- Nếu trả nhiều hơn số cần trả thì chỉ lấy đủ
    DECLARE @ActualPay DECIMAL(18,2) = @SoTien
    IF @SoTien > @ConLai SET @ActualPay = @ConLai
    
    -- Cập nhật số tiền bệnh nhân đã trả
    UPDATE HoaDon
    SET BenhNhanThanhToan = BenhNhanThanhToan + @ActualPay
    WHERE Id = @Id
    
    -- Nếu đã trả đủ thì chuyển trạng thái
    IF (SELECT BenhNhanThanhToan FROM HoaDon WHERE Id = @Id) >= @CanThanhToan
    BEGIN
        UPDATE HoaDon SET TrangThai = N'Đã thanh toán' WHERE Id = @Id
    END
    
    RETURN 1 -- Thanh toán thành công
END
GO
