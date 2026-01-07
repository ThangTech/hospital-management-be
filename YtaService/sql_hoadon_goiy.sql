-- =====================================================
-- SQL: GỢI Ý TIỀN VIỆN PHÍ (GIƯỜNG BỆNH)
-- =====================================================

CREATE OR ALTER PROCEDURE sp_HoaDon_GetGoiY
    @NhapVienId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        nv.Id AS NhapVienId,
        bn.Id AS BenhNhanId,
        bn.HoTen AS TenBenhNhan,
        bn.MucHuong,
        gb.TenGiuong,
        gb.GiaTien AS GiaGiuong,
        nv.NgayNhap,
        DATEDIFF(HOUR, nv.NgayNhap, GETDATE()) / 24.0 AS SoNgayNam, -- Tính theo giờ quy ra ngày để chính xác hơn
        -- Gợi ý: Số ngày (làm tròn lên 1 nếu < 1) * Giá giường
        (CASE WHEN DATEDIFF(DAY, nv.NgayNhap, GETDATE()) <= 0 THEN 1 ELSE DATEDIFF(DAY, nv.NgayNhap, GETDATE()) END * gb.GiaTien) AS TongTienSuggested
    FROM NhapVien nv
    JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    JOIN GiuongBenh gb ON nv.GiuongId = gb.Id
    WHERE nv.Id = @NhapVienId
END
GO
