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

    -- 2. Lấy thông tin BHYT chuyên sâu
    DECLARE @MucHuong DECIMAL(3,2), @SoTheBHYT NVARCHAR(50), @HanThe DATE, @IsDungTuyen BIT
    SELECT 
        @MucHuong = ISNULL(MucHuong, 0), 
        @SoTheBHYT = SoTheBaoHiem,
        @HanThe = HanTheBHYT
    FROM BenhNhan WHERE Id = @BenhNhanId

    SELECT @IsDungTuyen = ISNULL(IsDungTuyen, 1) FROM NhapVien WHERE Id = @NhapVienId

    -- 3. Logic tính toán BHYT chi trả (Khoa học)
    DECLARE @FinBaoHiem DECIMAL(18,2) = @BaoHiemChiTra_Manual
    
    IF @FinBaoHiem IS NULL -- Tự động tính toán
    BEGIN
        -- Kiểm tra thẻ còn hạn và có số thẻ
        IF @SoTheBHYT IS NOT NULL AND @HanThe >= GETDATE()
        BEGIN
            -- Nếu đúng tuyến hưởng 100% của mức hưởng, trái tuyến chỉ hưởng 40% (ví dụ)
            IF @IsDungTuyen = 1
                SET @FinBaoHiem = @TongTien * @MucHuong
            ELSE
                SET @FinBaoHiem = @TongTien * @MucHuong * 0.4 
        END
        ELSE
            SET @FinBaoHiem = 0 -- Hết hạn hoặc không có thẻ
    END

    -- 4. Tính số tiền bệnh nhân phải trả
    DECLARE @BenhNhanPhaiTra DECIMAL(18,2) = @TongTien - @FinBaoHiem
    
    -- 5. Xác định trạng thái: nếu BHYT cover hết thì đã thanh toán
    DECLARE @TrangThai NVARCHAR(50) = N'Chưa thanh toán'
    DECLARE @BenhNhanDaTra DECIMAL(18,2) = 0
    
    -- Nếu bệnh nhân không cần trả thêm (BHYT cover 100%) thì mark as paid
    IF @BenhNhanPhaiTra <= 0 
    BEGIN
        SET @TrangThai = N'Đã thanh toán'
        SET @BenhNhanDaTra = 0
    END
    ELSE
    BEGIN
        -- Với trường hợp xuất viện, giả định bệnh nhân đã trả hết số tiền còn lại
        -- (vì modal thanh toán chỉ được bấm sau khi họ "thanh toán")
        SET @TrangThai = N'Đã thanh toán'
        SET @BenhNhanDaTra = @BenhNhanPhaiTra
    END
    
    -- 6. Chèn hóa đơn
    INSERT INTO HoaDon (Id, BenhNhanId, NhapVienId, TongTien, BaoHiemChiTra, BenhNhanThanhToan, Ngay, TrangThai)
    VALUES (@Id, @BenhNhanId, @NhapVienId, @TongTien, @FinBaoHiem, @BenhNhanDaTra, GETDATE(), @TrangThai)
    
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
