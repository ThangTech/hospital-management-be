-- =====================================================
-- SQL: GỢI Ý TIỀN VIỆN PHÍ (FULL: Giường + Phẫu thuật + Xét nghiệm)
-- =====================================================

CREATE OR ALTER PROCEDURE sp_HoaDon_GetGoiY
    @NhapVienId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    -- Tính chi phí phẫu thuật (chỉ lấy đã hoàn thành)
    DECLARE @ChiPhiPhauThuat DECIMAL(18,2) = 0;
    SELECT @ChiPhiPhauThuat = ISNULL(SUM(ChiPhi), 0)
    FROM PhauThuat
    WHERE NhapVienId = @NhapVienId 
      AND (TrangThai = N'Đã hoàn thành' OR TrangThai = N'Hoàn thành' OR TrangThai = N'HoanThanh');

    -- Tính chi phí xét nghiệm
    DECLARE @ChiPhiXetNghiem DECIMAL(18,2) = 0;
    SELECT @ChiPhiXetNghiem = ISNULL(SUM(DonGia), 0)
    FROM XetNghiem
    WHERE NhapVienId = @NhapVienId;

    -- Tính tiền giường
    DECLARE @SoNgay INT;
    DECLARE @GiaGiuong DECIMAL(18,2);
    DECLARE @TienGiuong DECIMAL(18,2);
    
    SELECT 
        @GiaGiuong = ISNULL(gb.GiaTien, 0),
        @SoNgay = CASE WHEN DATEDIFF(DAY, nv.NgayNhap, GETDATE()) <= 0 THEN 1 ELSE DATEDIFF(DAY, nv.NgayNhap, GETDATE()) END
    FROM NhapVien nv
    JOIN GiuongBenh gb ON nv.GiuongId = gb.Id
    WHERE nv.Id = @NhapVienId;
    
    SET @TienGiuong = @SoNgay * @GiaGiuong;

    -- Tổng tiền = Giường + Phẫu thuật + Xét nghiệm
    DECLARE @TongTien DECIMAL(18,2) = @TienGiuong + @ChiPhiPhauThuat + @ChiPhiXetNghiem;

    SELECT 
        nv.Id AS NhapVienId,
        bn.Id AS BenhNhanId,
        bn.HoTen AS TenBenhNhan,
        ISNULL(bn.MucHuong, 0) AS MucHuong,
        ISNULL(gb.TenGiuong, N'Chưa đặt tên') AS TenGiuong,
        ISNULL(gb.GiaTien, 0) AS GiaGiuong,
        nv.NgayNhap,
        DATEDIFF(HOUR, nv.NgayNhap, GETDATE()) / 24.0 AS SoNgayNam,
        @TienGiuong AS TienGiuong,
        @ChiPhiPhauThuat AS ChiPhiPhauThuat,
        @ChiPhiXetNghiem AS ChiPhiXetNghiem,
        @TongTien AS TongTienSuggested
    FROM NhapVien nv
    JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    JOIN GiuongBenh gb ON nv.GiuongId = gb.Id
    WHERE nv.Id = @NhapVienId
END
GO
