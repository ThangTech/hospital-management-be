-- SQL Script: Thêm dữ liệu mẫu kiểm tra BHYT
-- Chạy script này trong SQL Server của bạn

-- 1. Xóa dữ liệu cũ nếu muốn (Cẩn thận: Sẽ xóa hết bệnh nhân hiện tại)
-- DELETE FROM BenhNhan;

-- 2. Thêm bệnh nhân mẫu 1: Cựu chiến binh (Mã HT, mức hưởng 100%, đúng tuyến)
INSERT INTO BenhNhan (Id, HoTen, NgaySinh, GioiTinh, DiaChi, SoTheBaoHiem, MucHuong, HanTheBHYT)
VALUES (
    NEWID(), 
    N'Cao Văn Bình', 
    '1950-05-15', 
    N'Nam', 
    N'Hà Nội', 
    'HT2010100100001', 
    1.0, 
    '2026-12-31'
);

-- 3. Thêm bệnh nhân mẫu 2: Trẻ em (Mã TE, mức hưởng 100%, đúng tuyến)
INSERT INTO BenhNhan (Id, HoTen, NgaySinh, GioiTinh, DiaChi, SoTheBaoHiem, MucHuong, HanTheBHYT)
VALUES (
    NEWID(), 
    N'Lê Gia Bảo', 
    '2020-01-20', 
    N'Nam', 
    N'TP.HCM', 
    'TE1797935000001', 
    1.0, 
    '2026-12-31'
);

-- 4. Thêm bệnh nhân mẫu 3: Hộ nghèo (Mã HN, mức hưởng 95%, trái tuyến)
INSERT INTO BenhNhan (Id, HoTen, NgaySinh, GioiTinh, DiaChi, SoTheBaoHiem, MucHuong, HanTheBHYT)
VALUES (
    NEWID(), 
    N'Phạm Thị Nghèo', 
    '1985-11-10', 
    N'Nữ', 
    N'Nghệ An', 
    'HN3404035000222', 
    0.95, 
    '2026-12-31'
);

-- 5. Thêm bệnh nhân mẫu 4: Đối tượng phổ thông (Mã GD, mức hưởng 80%, trái tuyến)
INSERT INTO BenhNhan (Id, HoTen, NgaySinh, GioiTinh, DiaChi, SoTheBaoHiem, MucHuong, HanTheBHYT)
VALUES (
    NEWID(), 
    N'Nguyễn Văn Chung', 
    '1992-03-25', 
    N'Nam', 
    N'Đồng Nai', 
    'GD4757535000888', 
    0.8, 
    '2026-12-31'
);

SELECT * FROM BenhNhan;
