-- =====================================================
-- SQL MIGRATION: VIỆT HÓA TOÀN BỘ TRẠNG THÁI
-- =====================================================

-- 1. Cập nhật trạng thái Giường bệnh
UPDATE GiuongBenh SET TrangThai = N'Trống' WHERE TrangThai = 'Available' OR TrangThai IS NULL;
UPDATE GiuongBenh SET TrangThai = N'Có người' WHERE TrangThai = 'Occupied';

-- 2. Cập nhật trạng thái Nhập viện
UPDATE NhapVien SET TrangThai = N'Đang điều trị' WHERE TrangThai = 'Active' OR TrangThai = 'In Treatment' OR TrangThai IS NULL;
UPDATE NhapVien SET TrangThai = N'Đã xuất viện' WHERE TrangThai = 'Discharged';

-- 3. Cập nhật trạng thái Hóa đơn
UPDATE HoaDon SET TrangThai = N'Chưa thanh toán' WHERE TrangThai = 'Unpaid' OR TrangThai IS NULL;
UPDATE HoaDon SET TrangThai = N'Đã thanh toán' WHERE TrangThai = 'Paid';

PRINT N'Đã chuyển đổi toàn bộ dữ liệu trạng thái sang Tiếng Việt!';
