-- =====================================================
-- SQL UPDATE: Bổ sung cột Mức hưởng BHYT
-- =====================================================

-- 1. Thêm cột MucHuong vào bảng BenhNhan (Nếu chưa có)
-- MucHuong: Giá trị từ 0 đến 1 (ví dụ: 0.8 là hưởng 80%)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('BenhNhan') AND name = 'MucHuong')
BEGIN
    ALTER TABLE BenhNhan ADD MucHuong DECIMAL(3,2) DEFAULT 0.0;
    PRINT N'Đã thêm cột MucHuong vào bảng BenhNhan.';
END
GO

-- 2. Cập nhật dữ liệu mẫu (Giả sử mặc định là 80% cho những người có thẻ)
UPDATE BenhNhan 
SET MucHuong = 0.8 
WHERE SoTheBaoHiem IS NOT NULL AND (MucHuong IS NULL OR MucHuong = 0);
GO

PRINT N'Cập nhật cấu trúc bảng BenhNhan thành công!';
