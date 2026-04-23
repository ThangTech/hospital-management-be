-- =====================================================
-- Soft Delete cho BenhNhan
-- =====================================================

USE [hospital_manage]
GO

-- Thêm cột DaXoa vào bảng BenhNhan
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Object_ID = Object_ID('dbo.BenhNhan') AND Name = 'DaXoa')
BEGIN
    ALTER TABLE [dbo].[BenhNhan] ADD [DaXoa] [bit] NOT NULL DEFAULT (0)
END
GO

-- =====================================================
-- Sửa sp_BenhNhan_GetAll - chỉ lấy bản ghi chưa xóa
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE Name = 'sp_BenhNhan_GetAll')
    DROP PROCEDURE [dbo].[sp_BenhNhan_GetAll]
GO

CREATE PROCEDURE [dbo].[sp_BenhNhan_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [dbo].[BenhNhan] WHERE [DaXoa] = 0
END
GO

-- =====================================================
-- Sửa sp_BenhNhan_GetById - chỉ lấy bản ghi chưa xóa
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE Name = 'sp_BenhNhan_GetById')
    DROP PROCEDURE [dbo].[sp_BenhNhan_GetById]
GO

CREATE PROCEDURE [dbo].[sp_BenhNhan_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [dbo].[BenhNhan] WHERE [Id] = @Id AND [DaXoa] = 0
END
GO

-- =====================================================
-- Sửa sp_BenhNhan_Search - chỉ tìm bản ghi chưa xóa
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE Name = 'sp_BenhNhan_Search')
    DROP PROCEDURE [dbo].[sp_BenhNhan_Search]
GO

CREATE PROCEDURE [dbo].[sp_BenhNhan_Search]
    @PageIndex INT = 1,
    @PageSize INT = 10,
    @HoTen NVARCHAR(255) = NULL,
    @DiaChi NVARCHAR(255) = NULL,
    @SoTheBaoHiem NVARCHAR(50) = NULL,
    @TotalRecord INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Đếm tổng
    SELECT @TotalRecord = COUNT(*) FROM [dbo].[BenhNhan]
    WHERE [DaXoa] = 0
        AND (@HoTen IS NULL OR @HoTen = '' OR [HoTen] LIKE '%' + @HoTen + '%')
        AND (@DiaChi IS NULL OR @DiaChi = '' OR [DiaChi] LIKE '%' + @DiaChi + '%')
        AND (@SoTheBaoHiem IS NULL OR @SoTheBaoHiem = '' OR [SoTheBaoHiem] LIKE '%' + @SoTheBaoHiem + '%')

    -- Phân trang
    DECLARE @StartIndex INT = (@PageIndex - 1) * @PageSize + 1
    DECLARE @EndIndex INT = @PageIndex * @PageSize

    SELECT * FROM (
        SELECT ROW_NUMBER() OVER (ORDER BY [NgaySinh] DESC) AS RowNum, *
        FROM [dbo].[BenhNhan]
        WHERE [DaXoa] = 0
            AND (@HoTen IS NULL OR @HoTen = '' OR [HoTen] LIKE '%' + @HoTen + '%')
            AND (@DiaChi IS NULL OR @DiaChi = '' OR [DiaChi] LIKE '%' + @DiaChi + '%')
            AND (@SoTheBaoHiem IS NULL OR @SoTheBaoHiem = '' OR [SoTheBaoHiem] LIKE '%' + @SoTheBaoHiem + '%')
    ) AS Result
    WHERE RowNum BETWEEN @StartIndex AND @EndIndex
END
GO

-- =====================================================
-- Sửa sp_BenhNhan_Delete - soft delete thay vì hard delete
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE Name = 'sp_BenhNhan_Delete')
    DROP PROCEDURE [dbo].[sp_BenhNhan_Delete]
GO

CREATE PROCEDURE [dbo].[sp_BenhNhan_Delete]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    -- Soft delete: chỉ SET DaXoa = 1 thay vì DELETE
    UPDATE [dbo].[BenhNhan] SET [DaXoa] = 1 WHERE [Id] = @Id
END
GO

-- =====================================================
-- Thêm sp_BenhNhan_Restore - khôi phục bản ghi đã xóa
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE Name = 'sp_BenhNhan_Restore')
    DROP PROCEDURE [dbo].[sp_BenhNhan_Restore]
GO

CREATE PROCEDURE [dbo].[sp_BenhNhan_Restore]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[BenhNhan] SET [DaXoa] = 0 WHERE [Id] = @Id
END
GO

-- =====================================================
-- Thêm sp_BenhNhan_GetDeleted - xem các bản ghi đã xóa
-- =====================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE Name = 'sp_BenhNhan_GetDeleted')
    DROP PROCEDURE [dbo].[sp_BenhNhan_GetDeleted]
GO

CREATE PROCEDURE [dbo].[sp_BenhNhan_GetDeleted]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [dbo].[BenhNhan] WHERE [DaXoa] = 1
END
GO
