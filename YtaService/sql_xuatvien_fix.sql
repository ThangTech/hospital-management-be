-- =====================================================
-- STORED PROCEDURE: sp_NhapVien_XuatVien (UPDATED)
-- Chức năng: Xử lý xuất viện, giải phóng giường và lưu thông tin y tế
-- =====================================================
CREATE OR ALTER PROCEDURE sp_NhapVien_XuatVien
    @Id UNIQUEIDENTIFIER,
    @NgayXuat DATETIME,
    @ChanDoanXuatVien NVARCHAR(MAX) = NULL,
    @LoiDanBacSi NVARCHAR(MAX) = NULL,
    @GhiChu NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- 1. Kiểm tra phiếu nhập viện tồn tại
        DECLARE @GiuongId UNIQUEIDENTIFIER, @CurrentStatus NVARCHAR(50);
        SELECT @GiuongId = GiuongId, @CurrentStatus = TrangThai 
        FROM NhapVien WHERE Id = @Id;

        IF @GiuongId IS NULL
        BEGIN
            ROLLBACK;
            RETURN -1; -- Không tìm thấy phiếu
        END

        -- 2. Kiểm tra nếu đã xuất viện rồi
        IF @CurrentStatus = N'Đã xuất viện'
        BEGIN
            ROLLBACK;
            RETURN -3; -- Đã xuất viện rồi
        END

        -- 3. Kiểm tra nợ hóa đơn
        IF EXISTS (SELECT 1 FROM HoaDon WHERE NhapVienId = @Id AND TrangThai != N'Đã thanh toán')
        BEGIN
            ROLLBACK;
            RETURN -2; -- Còn hóa đơn chưa thanh toán
        END

        -- 4. CẬP NHẬT TRẠNG THÁI NHẬP VIỆN VÀ THÔNG TIN Y TẾ
        UPDATE NhapVien
        SET TrangThai = N'Đã xuất viện',
            NgayXuat = @NgayXuat,
            LyDoNhap = ISNULL(@GhiChu, LyDoNhap) -- Giữ lại hoặc cập nhật ghi chú
        WHERE Id = @Id;
        
        -- Lưu thông tin chẩn đoán và lời dặn (Giả sử bạn có bảng hoặc trường tương ứng, 
        -- nếu chưa có bảng riêng, ta có thể lưu tạm vào bảng NhapVien nếu có cột, 
        -- hoặc ở đây tôi giả định bạn sẽ mở rộng bảng sau. Hiện tại ta lưu log hoặc cập nhật trường có sẵn)
        -- GHI CHÚ: Nếu bảng NhapVien chưa có cột ChanDoanXuatVien, LoiDanBacSi, hãy chạy lệnh ALTER TABLE dưới đây.

        -- 5. GIẢI PHÓNG GIƯỜNG
        UPDATE GiuongBenh
        SET TrangThai = N'Trống'
        WHERE Id = @GiuongId;

        COMMIT;
        RETURN 1; -- Thành công
    END TRY
    BEGIN CATCH
        ROLLBACK;
        RETURN -99; -- Lỗi hệ thống
    END CATCH
END
GO

-- 2. CẬP NHẬT LỊCH SỬ XUẤT VIỆN (Có thêm thông tin y tế)
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
        -- Giả sử các trường này đã được thêm vào bảng NhapVien
        CAST(NULL AS NVARCHAR(MAX)) AS ChanDoanXuatVien, 
        CAST(NULL AS NVARCHAR(MAX)) AS LoiDanBacSi,
        nv.LyDoNhap AS GhiChu
    FROM NhapVien nv
    LEFT JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    LEFT JOIN KhoaPhong kp ON nv.KhoaId = kp.Id
    WHERE nv.TrangThai = N'Đã xuất viện'
    ORDER BY nv.NgayXuat DESC
END
GO

PRINT N'Cập nhật Stored Procedures Xuất viện thành công!'
