-- =====================================================
-- STORED PROCEDURES CHO MODULE BỆNH NHÂN (BenhNhan)
-- =====================================================

-- 1. LẤY TẤT CẢ BỆNH NHÂN
CREATE OR ALTER PROCEDURE sp_BenhNhan_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM BenhNhan ORDER BY HoTen;
END
GO

-- 2. LẤY CHI TIẾT BỆNH NHÂN THEO ID
CREATE OR ALTER PROCEDURE sp_BenhNhan_GetById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM BenhNhan WHERE Id = @Id;
END
GO

-- 3. THÊM BỆNH NHÂN MỚI
CREATE OR ALTER PROCEDURE sp_BenhNhan_Create
    @Id UNIQUEIDENTIFIER,
    @HoTen NVARCHAR(255),
    @NgaySinh DATE,
    @GioiTinh NVARCHAR(50),
    @DiaChi NVARCHAR(255),
    @SoTheBaoHiem NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO BenhNhan (Id, HoTen, NgaySinh, GioiTinh, DiaChi, SoTheBaoHiem)
    VALUES (@Id, @HoTen, @NgaySinh, @GioiTinh, @DiaChi, @SoTheBaoHiem);
END
GO

-- 4. CẬP NHẬT THÔNG TIN BỆNH NHÂN
CREATE OR ALTER PROCEDURE sp_BenhNhan_Update
    @Id UNIQUEIDENTIFIER,
    @HoTen NVARCHAR(255),
    @NgaySinh DATE,
    @GioiTinh NVARCHAR(50),
    @DiaChi NVARCHAR(255),
    @SoTheBaoHiem NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE BenhNhan
    SET HoTen = @HoTen,
        NgaySinh = @NgaySinh,
        GioiTinh = @GioiTinh,
        DiaChi = @DiaChi,
        SoTheBaoHiem = @SoTheBaoHiem
    WHERE Id = @Id;
END
GO

-- 5. XÓA BỆNH NHÂN
CREATE OR ALTER PROCEDURE sp_BenhNhan_Delete
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Xóa các hóa đơn liên quan nếu có (Chỉ nên dùng nếu đã qua validation ở BLL)
    -- Hoặc chỉ đơn giản là DELETE nếu DB có CASCADE (Cẩn thận!)
    DELETE FROM BenhNhan WHERE Id = @Id;
END
GO

-- 6. TÌM KIẾM BỆNH NHÂN PHÂN TRANG
CREATE OR ALTER PROCEDURE sp_BenhNhan_Search
    @PageIndex INT = 1,
    @PageSize INT = 10,
    @HoTen NVARCHAR(255) = NULL,
    @DiaChi NVARCHAR(255) = NULL,
    @SoTheBaoHiem NVARCHAR(50) = NULL,
    @TotalRecord INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Tính tổng số bản ghi
    SELECT @TotalRecord = COUNT(*) 
    FROM BenhNhan
    WHERE (@HoTen IS NULL OR HoTen LIKE '%' + @HoTen + '%')
      AND (@DiaChi IS NULL OR DiaChi LIKE '%' + @DiaChi + '%')
      AND (@SoTheBaoHiem IS NULL OR SoTheBaoHiem LIKE '%' + @SoTheBaoHiem + '%');

    -- Lấy dữ liệu phân trang
    SELECT * 
    FROM BenhNhan
    WHERE (@HoTen IS NULL OR HoTen LIKE '%' + @HoTen + '%')
      AND (@DiaChi IS NULL OR DiaChi LIKE '%' + @DiaChi + '%')
      AND (@SoTheBaoHiem IS NULL OR SoTheBaoHiem LIKE '%' + @SoTheBaoHiem + '%')
    ORDER BY HoTen
    OFFSET (@PageIndex - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
