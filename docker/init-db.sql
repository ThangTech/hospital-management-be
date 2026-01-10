-- =====================================================
-- Hospital Management Database Initialization Script
-- For Docker SQL Server Container
-- =====================================================

-- Wait for SQL Server to be ready
PRINT 'Initializing hospital_manage database...'
GO

-- Create database if not exists
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'hospital_manage')
BEGIN
    CREATE DATABASE [hospital_manage]
END
GO

USE [hospital_manage]
GO

-- =====================================================
-- CREATE TABLES
-- =====================================================

-- Table: NguoiDung (Users - must be created first for FK references)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NguoiDung' AND xtype='U')
CREATE TABLE [dbo].[NguoiDung](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [TenDangNhap] [nvarchar](255) NULL,
    [MatKhauHash] [nvarchar](255) NULL,
    [VaiTro] [nvarchar](50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Table: KhoaPhong (Departments)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KhoaPhong' AND xtype='U')
CREATE TABLE [dbo].[KhoaPhong](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [TenKhoa] [nvarchar](255) NULL,
    [LoaiKhoa] [nvarchar](50) NULL,
    [SoGiuongTieuChuan] [int] NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Table: BenhNhan (Patients)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BenhNhan' AND xtype='U')
CREATE TABLE [dbo].[BenhNhan](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [HoTen] [nvarchar](255) NULL,
    [NgaySinh] [date] NULL,
    [GioiTinh] [nvarchar](50) NULL,
    [DiaChi] [nvarchar](255) NULL,
    [SoTheBaoHiem] [nvarchar](50) NULL,
    [MucHuong] [decimal](3, 2) NULL DEFAULT ((0.0)),
    [HanTheBHYT] [datetime] NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Table: BacSi (Doctors)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BacSi' AND xtype='U')
CREATE TABLE [dbo].[BacSi](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [HoTen] [nvarchar](255) NULL,
    [ChuyenKhoa] [nvarchar](255) NULL,
    [ThongTinLienHe] [nvarchar](255) NULL,
    [KhoaId] [uniqueidentifier] NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY([KhoaId]) REFERENCES [dbo].[KhoaPhong] ([Id])
)
GO

-- Table: DieuDuong (Nurses)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DieuDuong' AND xtype='U')
CREATE TABLE [dbo].[DieuDuong](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [HoTen] [nvarchar](255) NULL,
    [KhoaId] [uniqueidentifier] NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY([KhoaId]) REFERENCES [dbo].[KhoaPhong] ([Id])
)
GO

-- Table: YTa (Medical Assistants)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='YTa' AND xtype='U')
CREATE TABLE [dbo].[YTa](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [HoTen] [nvarchar](255) NOT NULL,
    [NgaySinh] [date] NULL,
    [GioiTinh] [nvarchar](10) NULL,
    [SoDienThoai] [nvarchar](20) NULL,
    [KhoaId] [uniqueidentifier] NULL,
    [ChungChiHanhNghe] [nvarchar](50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY([KhoaId]) REFERENCES [dbo].[KhoaPhong] ([Id])
)
GO

-- Table: GiuongBenh (Hospital Beds)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='GiuongBenh' AND xtype='U')
CREATE TABLE [dbo].[GiuongBenh](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [KhoaId] [uniqueidentifier] NULL,
    [LoaiGiuong] [nvarchar](50) NULL,
    [TrangThai] [nvarchar](50) NULL,
    [GiaTien] [decimal](18, 2) NULL DEFAULT ((0)),
    [TenGiuong] [nvarchar](50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY([KhoaId]) REFERENCES [dbo].[KhoaPhong] ([Id])
)
GO

-- Table: NhapVien (Admissions)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NhapVien' AND xtype='U')
CREATE TABLE [dbo].[NhapVien](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [BenhNhanId] [uniqueidentifier] NULL,
    [NgayNhap] [datetime] NULL,
    [NgayXuat] [datetime] NULL,
    [KhoaId] [uniqueidentifier] NULL,
    [LyDoNhap] [nvarchar](255) NULL,
    [TrangThai] [nvarchar](50) NULL,
    [GiuongId] [uniqueidentifier] NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY([BenhNhanId]) REFERENCES [dbo].[BenhNhan] ([Id]),
    FOREIGN KEY([KhoaId]) REFERENCES [dbo].[KhoaPhong] ([Id]),
    FOREIGN KEY([GiuongId]) REFERENCES [dbo].[GiuongBenh] ([Id])
)
GO

-- Table: HoSoBenhAn (Medical Records)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='HoSoBenhAn' AND xtype='U')
CREATE TABLE [dbo].[HoSoBenhAn](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [NhapVienId] [uniqueidentifier] NULL,
    [TienSuBenh] [nvarchar](max) NULL,
    [ChanDoanBanDau] [nvarchar](max) NULL,
    [ChanDoanRaVien] [nvarchar](max) NULL,
    [PhuongAnDieuTri] [nvarchar](max) NULL,
    [KetQuaDieuTri] [nvarchar](255) NULL,
    [NgayLap] [datetime] NULL DEFAULT (getdate()),
    [BacSiPhuTrachId] [uniqueidentifier] NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY([NhapVienId]) REFERENCES [dbo].[NhapVien] ([Id]),
    FOREIGN KEY([BacSiPhuTrachId]) REFERENCES [dbo].[BacSi] ([Id])
)
GO

-- Table: Audit_HoSoBenhAn (Medical Records Audit)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Audit_HoSoBenhAn' AND xtype='U')
CREATE TABLE [dbo].[Audit_HoSoBenhAn](
    [Id] [uniqueidentifier] NOT NULL,
    [HoSoBenhAnId] [uniqueidentifier] NULL,
    [HanhDong] [nvarchar](50) NULL,
    [ChanDoanCu] [nvarchar](max) NULL,
    [KetQuaCu] [nvarchar](255) NULL,
    [NguoiSua] [nvarchar](100) NULL,
    [ThoiGianSua] [datetime] NULL DEFAULT (getdate()),
    [NguoiDungId] [uniqueidentifier] NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY([NguoiDungId]) REFERENCES [dbo].[NguoiDung] ([Id])
)
GO

-- Table: DichVuDieuTri (Treatment Services)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DichVuDieuTri' AND xtype='U')
CREATE TABLE [dbo].[DichVuDieuTri](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [NhapVienId] [uniqueidentifier] NULL,
    [LoaiDichVu] [nvarchar](255) NULL,
    [SoLuong] [int] NULL,
    [Ngay] [datetime] NULL,
    [BacSiId] [uniqueidentifier] NULL,
    [DieuDuongId] [uniqueidentifier] NULL,
    [DonGia] [decimal](18, 2) NULL DEFAULT ((0)),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY([NhapVienId]) REFERENCES [dbo].[NhapVien] ([Id]),
    FOREIGN KEY([BacSiId]) REFERENCES [dbo].[BacSi] ([Id]),
    FOREIGN KEY([DieuDuongId]) REFERENCES [dbo].[DieuDuong] ([Id])
)
GO

-- Table: HoaDon (Invoices)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='HoaDon' AND xtype='U')
CREATE TABLE [dbo].[HoaDon](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [BenhNhanId] [uniqueidentifier] NULL,
    [NhapVienId] [uniqueidentifier] NULL,
    [TongTien] [decimal](18, 2) NULL,
    [BaoHiemChiTra] [decimal](18, 2) NULL,
    [BenhNhanThanhToan] [decimal](18, 2) NULL,
    [Ngay] [datetime] NULL,
    [TrangThai] [nvarchar](50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY([BenhNhanId]) REFERENCES [dbo].[BenhNhan] ([Id]),
    FOREIGN KEY([NhapVienId]) REFERENCES [dbo].[NhapVien] ([Id])
)
GO

-- Table: NhatKyHeThong (System Logs)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NhatKyHeThong' AND xtype='U')
CREATE TABLE [dbo].[NhatKyHeThong](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [NguoiDungId] [uniqueidentifier] NULL,
    [HanhDong] [nvarchar](255) NULL,
    [ThoiGian] [datetime] NULL,
    [MoTa] [nvarchar](255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY([NguoiDungId]) REFERENCES [dbo].[NguoiDung] ([Id])
)
GO

-- Table: PhauThuat (Surgeries)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PhauThuat' AND xtype='U')
CREATE TABLE [dbo].[PhauThuat](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [NhapVienId] [uniqueidentifier] NULL,
    [LoaiPhauThuat] [nvarchar](255) NULL,
    [BacSiChinhId] [uniqueidentifier] NULL,
    [Ekip] [nvarchar](255) NULL,
    [Ngay] [datetime] NULL,
    [PhongMo] [nvarchar](50) NULL,
    [TrangThai] [nvarchar](50) NULL,
    [ChiPhi] [decimal](18, 2) NULL DEFAULT ((0)),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY([NhapVienId]) REFERENCES [dbo].[NhapVien] ([Id]),
    FOREIGN KEY([BacSiChinhId]) REFERENCES [dbo].[BacSi] ([Id])
)
GO

-- Table: XetNghiem (Lab Tests)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='XetNghiem' AND xtype='U')
CREATE TABLE [dbo].[XetNghiem](
    [Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
    [NhapVienId] [uniqueidentifier] NULL,
    [LoaiXetNghiem] [nvarchar](255) NULL,
    [KetQua] [nvarchar](255) NULL,
    [Ngay] [datetime] NULL,
    [BacSiId] [uniqueidentifier] NULL,
    [DonGia] [decimal](18, 2) NULL DEFAULT ((0)),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY([NhapVienId]) REFERENCES [dbo].[NhapVien] ([Id]),
    FOREIGN KEY([BacSiId]) REFERENCES [dbo].[BacSi] ([Id])
)
GO

-- =====================================================
-- INSERT DEFAULT ADMIN USER
-- Password: admin123 (hashed)
-- =====================================================
IF NOT EXISTS (SELECT * FROM [dbo].[NguoiDung] WHERE [TenDangNhap] = 'admin')
BEGIN
    INSERT INTO [dbo].[NguoiDung] ([Id], [TenDangNhap], [MatKhauHash], [VaiTro])
    VALUES (NEWID(), 'admin', 'AQAAAAIAAYagAAAAELDpzLQ8QqZJKGJ0k8KVKV0lL9mJKQ3k0VJF+KVKV0lL9mJKQ3k0VJF+KVKV0lL9mJKQ3k0VJF+', 'Admin')
END
GO

-- =====================================================
-- STORED PROCEDURES
-- =====================================================

-- sp_GetNguoiDungByTenDangNhapFull (Required for Login)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetNguoiDungByTenDangNhapFull')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetNguoiDungByTenDangNhapFull]
    @TenDangNhap NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, TenDangNhap, MatKhauHash, VaiTro 
    FROM NguoiDung 
    WHERE TenDangNhap = @TenDangNhap;
END
')
GO

-- sp_GetNguoiDungByIdFull
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetNguoiDungByIdFull')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetNguoiDungByIdFull]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, TenDangNhap, MatKhauHash, VaiTro 
    FROM NguoiDung 
    WHERE Id = @Id;
END
')
GO

-- sp_CreateNguoiDungFull
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CreateNguoiDungFull')
EXEC('
CREATE PROCEDURE [dbo].[sp_CreateNguoiDungFull]
    @Id UNIQUEIDENTIFIER,
    @TenDangNhap NVARCHAR(100),
    @MatKhauHash NVARCHAR(256),
    @VaiTro NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO NguoiDung (Id, TenDangNhap, MatKhauHash, VaiTro)
    OUTPUT INSERTED.Id, INSERTED.TenDangNhap, INSERTED.MatKhauHash, INSERTED.VaiTro
    VALUES (@Id, @TenDangNhap, @MatKhauHash, @VaiTro);
END
')
GO

-- sp_GetAllNguoiDungFull
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetAllNguoiDungFull')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetAllNguoiDungFull]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, TenDangNhap, MatKhauHash, VaiTro FROM NguoiDung;
END
')
GO

-- sp_GetAllNguoiDung
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetAllNguoiDung')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetAllNguoiDung]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, TenDangNhap, VaiTro FROM NguoiDung ORDER BY TenDangNhap;
END
')
GO

-- sp_GetNguoiDungById
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetNguoiDungById')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetNguoiDungById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, TenDangNhap, VaiTro FROM NguoiDung WHERE Id = @Id;
END
')
GO

-- sp_CheckNguoiDungExists
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CheckNguoiDungExists')
EXEC('
CREATE PROCEDURE [dbo].[sp_CheckNguoiDungExists]
    @TenDangNhap NVARCHAR(100),
    @Exists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM NguoiDung WHERE TenDangNhap = @TenDangNhap)
        SET @Exists = 1;
    ELSE
        SET @Exists = 0;
END
')
GO

-- sp_SearchNguoiDung
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_SearchNguoiDung')
EXEC('
CREATE PROCEDURE [dbo].[sp_SearchNguoiDung]
    @TenDangNhap NVARCHAR(100) = NULL,
    @VaiTro NVARCHAR(50) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @TotalRecords = COUNT(*) FROM NguoiDung
    WHERE (@TenDangNhap IS NULL OR TenDangNhap LIKE ''%'' + @TenDangNhap + ''%'')
      AND (@VaiTro IS NULL OR VaiTro = @VaiTro);
    SELECT Id, TenDangNhap, VaiTro FROM NguoiDung
    WHERE (@TenDangNhap IS NULL OR TenDangNhap LIKE ''%'' + @TenDangNhap + ''%'')
      AND (@VaiTro IS NULL OR VaiTro = @VaiTro)
    ORDER BY TenDangNhap
    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END
')
GO

-- sp_UpdateNguoiDung
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_UpdateNguoiDung')
EXEC('
CREATE PROCEDURE [dbo].[sp_UpdateNguoiDung]
    @Id UNIQUEIDENTIFIER,
    @TenDangNhap NVARCHAR(100) = NULL,
    @VaiTro NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE NguoiDung SET TenDangNhap = COALESCE(@TenDangNhap, TenDangNhap),
        VaiTro = COALESCE(@VaiTro, VaiTro) WHERE Id = @Id;
    SELECT Id, TenDangNhap, VaiTro FROM NguoiDung WHERE Id = @Id;
END
')
GO

-- sp_DeleteNguoiDung
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_DeleteNguoiDung')
EXEC('
CREATE PROCEDURE [dbo].[sp_DeleteNguoiDung]
    @Id UNIQUEIDENTIFIER,
    @RowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM NguoiDung WHERE Id = @Id;
    SET @RowsAffected = @@ROWCOUNT;
END
')
GO

-- sp_ResetPassword
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ResetPassword')
EXEC('
CREATE PROCEDURE [dbo].[sp_ResetPassword]
    @Id UNIQUEIDENTIFIER,
    @MatKhauHash NVARCHAR(256),
    @RowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE NguoiDung SET MatKhauHash = @MatKhauHash WHERE Id = @Id;
    SET @RowsAffected = @@ROWCOUNT;
END
')
GO

-- sp_GetAllDoctors
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetAllDoctors')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetAllDoctors]
AS
BEGIN 
    SELECT b.Id, b.HoTen, b.ChuyenKhoa, b.ThongTinLienHe, b.KhoaId, k.TenKhoa
    FROM BacSi b LEFT JOIN KhoaPhong k ON b.KhoaId = k.Id
END
')
GO

-- sp_GetDoctorById
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetDoctorById')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetDoctorById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SELECT b.Id, b.HoTen, b.ChuyenKhoa, b.ThongTinLienHe, b.KhoaId, k.TenKhoa
    FROM BacSi b LEFT JOIN KhoaPhong k ON b.KhoaId = k.Id WHERE b.Id = @Id
END
')
GO

-- sp_GetAllLabTests
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetAllLabTests')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetAllLabTests]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT xn.Id, xn.NhapVienId, xn.BacSiId, xn.LoaiXetNghiem, xn.KetQua, xn.Ngay, xn.DonGia,
        bn.HoTen AS TenBenhNhan, bn.NgaySinh AS NgaySinhBenhNhan, bn.Id AS BenhNhanId
    FROM XetNghiem xn
    INNER JOIN NhapVien nv ON xn.NhapVienId = nv.Id
    INNER JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    ORDER BY xn.Ngay DESC;
END
')
GO

-- sp_GetAllMedicalRecords
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetAllMedicalRecords')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetAllMedicalRecords]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT hs.Id, hs.NhapVienId, hs.TienSuBenh, hs.ChanDoanBanDau, hs.ChanDoanRaVien,
        hs.PhuongAnDieuTri, hs.KetQuaDieuTri, hs.NgayLap, hs.BacSiPhuTrachId,
        bn.HoTen AS TenBenhNhan, bn.NgaySinh AS NgaySinhBenhNhan, bn.Id AS BenhNhanId
    FROM HoSoBenhAn hs
    INNER JOIN NhapVien nv ON hs.NhapVienId = nv.Id
    INNER JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    ORDER BY hs.NgayLap DESC;
END
')
GO

-- sp_GetAllSurgeries
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetAllSurgeries')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetAllSurgeries]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT pt.Id, pt.NhapVienId, pt.BacSiChinhId, pt.LoaiPhauThuat, pt.Ekip, pt.Ngay,
        pt.PhongMo, pt.TrangThai, bn.HoTen AS TenBenhNhan, bn.NgaySinh AS NgaySinhBenhNhan, bn.Id AS BenhNhanId
    FROM PhauThuat pt
    INNER JOIN NhapVien nv ON pt.NhapVienId = nv.Id
    INNER JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    ORDER BY pt.Ngay DESC;
END
')
GO

-- sp_GetNhatKyHeThong
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetNhatKyHeThong')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetNhatKyHeThong]
    @NguoiDungId UNIQUEIDENTIFIER = NULL,
    @HanhDong NVARCHAR(200) = NULL,
    @TuNgay DATETIME = NULL,
    @DenNgay DATETIME = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @TotalRecords = COUNT(*) FROM NhatKyHeThong nk
    LEFT JOIN NguoiDung nd ON nk.NguoiDungId = nd.Id
    WHERE (@NguoiDungId IS NULL OR nk.NguoiDungId = @NguoiDungId)
      AND (@HanhDong IS NULL OR nk.HanhDong LIKE ''%'' + @HanhDong + ''%'')
      AND (@TuNgay IS NULL OR nk.ThoiGian >= @TuNgay)
      AND (@DenNgay IS NULL OR nk.ThoiGian <= @DenNgay);
    SELECT nk.Id, nk.NguoiDungId, nd.TenDangNhap AS TenNguoiDung, nk.HanhDong, nk.ThoiGian, nk.MoTa
    FROM NhatKyHeThong nk LEFT JOIN NguoiDung nd ON nk.NguoiDungId = nd.Id
    WHERE (@NguoiDungId IS NULL OR nk.NguoiDungId = @NguoiDungId)
      AND (@HanhDong IS NULL OR nk.HanhDong LIKE ''%'' + @HanhDong + ''%'')
      AND (@TuNgay IS NULL OR nk.ThoiGian >= @TuNgay)
      AND (@DenNgay IS NULL OR nk.ThoiGian <= @DenNgay)
    ORDER BY nk.ThoiGian DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END
')
GO

-- sp_GetAuditHoSoBenhAn
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetAuditHoSoBenhAn')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetAuditHoSoBenhAn]
    @NguoiDungId UNIQUEIDENTIFIER = NULL,
    @HanhDong NVARCHAR(200) = NULL,
    @TuNgay DATETIME = NULL,
    @DenNgay DATETIME = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @TotalRecords = COUNT(*) FROM Audit_HoSoBenhAn a
    LEFT JOIN NguoiDung nd ON a.NguoiDungId = nd.Id
    WHERE (@NguoiDungId IS NULL OR a.NguoiDungId = @NguoiDungId)
      AND (@HanhDong IS NULL OR a.HanhDong LIKE ''%'' + @HanhDong + ''%'')
      AND (@TuNgay IS NULL OR a.ThoiGianSua >= @TuNgay)
      AND (@DenNgay IS NULL OR a.ThoiGianSua <= @DenNgay);
    SELECT a.Id, a.HoSoBenhAnId, a.HanhDong, a.ChanDoanCu, a.KetQuaCu, 
           a.NguoiDungId, COALESCE(nd.TenDangNhap, a.NguoiSua) AS TenNguoiSua, a.ThoiGianSua
    FROM Audit_HoSoBenhAn a LEFT JOIN NguoiDung nd ON a.NguoiDungId = nd.Id
    WHERE (@NguoiDungId IS NULL OR a.NguoiDungId = @NguoiDungId)
      AND (@HanhDong IS NULL OR a.HanhDong LIKE ''%'' + @HanhDong + ''%'')
      AND (@TuNgay IS NULL OR a.ThoiGianSua >= @TuNgay)
      AND (@DenNgay IS NULL OR a.ThoiGianSua <= @DenNgay)
    ORDER BY a.ThoiGianSua DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END
')
GO

-- sp_GetBedCapacityReport
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetBedCapacityReport')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetBedCapacityReport]
    @KhoaId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT kp.Id AS KhoaId, kp.TenKhoa, COUNT(gb.Id) AS TongGiuong,
        SUM(CASE WHEN gb.TrangThai = N''Đang sử dụng'' OR gb.TrangThai = ''DangSuDung'' THEN 1 ELSE 0 END) AS GiuongDangSuDung,
        SUM(CASE WHEN gb.TrangThai = N''Trống'' OR gb.TrangThai = ''Trong'' OR gb.TrangThai IS NULL THEN 1 ELSE 0 END) AS GiuongTrong
    FROM KhoaPhong kp LEFT JOIN GiuongBenh gb ON kp.Id = gb.KhoaId
    WHERE (@KhoaId IS NULL OR kp.Id = @KhoaId)
    GROUP BY kp.Id, kp.TenKhoa ORDER BY kp.TenKhoa;
END
')
GO

-- sp_GetTreatmentCostByDepartment
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetTreatmentCostByDepartment')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetTreatmentCostByDepartment]
    @KhoaId UNIQUEIDENTIFIER = NULL,
    @TuNgay DATETIME = NULL,
    @DenNgay DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT kp.Id AS KhoaId, kp.TenKhoa,
        ISNULL(SUM(dv.DonGia * dv.SoLuong), 0) AS TongChiPhiDichVu,
        ISNULL(SUM(pt.ChiPhi), 0) AS TongChiPhiPhauThuat,
        ISNULL(SUM(xn.DonGia), 0) AS TongChiPhiXetNghiem,
        COUNT(DISTINCT nv.Id) AS SoLuotDieuTri
    FROM KhoaPhong kp
    LEFT JOIN NhapVien nv ON kp.Id = nv.KhoaId
        AND (@TuNgay IS NULL OR nv.NgayNhap >= @TuNgay)
        AND (@DenNgay IS NULL OR nv.NgayNhap <= @DenNgay)
    LEFT JOIN DichVuDieuTri dv ON nv.Id = dv.NhapVienId
    LEFT JOIN PhauThuat pt ON nv.Id = pt.NhapVienId
    LEFT JOIN XetNghiem xn ON nv.Id = xn.NhapVienId
    WHERE (@KhoaId IS NULL OR kp.Id = @KhoaId)
    GROUP BY kp.Id, kp.TenKhoa ORDER BY kp.TenKhoa;
END
')
GO

-- sp_GetTreatmentCostByServiceType
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetTreatmentCostByServiceType')
EXEC('
CREATE PROCEDURE [dbo].[sp_GetTreatmentCostByServiceType]
    @KhoaId UNIQUEIDENTIFIER = NULL,
    @TuNgay DATETIME = NULL,
    @DenNgay DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT LoaiDichVu, SUM(ISNULL(DonGia, 0) * ISNULL(SoLuong, 1)) AS TongChiPhi,
        SUM(ISNULL(SoLuong, 1)) AS SoLuong
    FROM DichVuDieuTri dv INNER JOIN NhapVien nv ON dv.NhapVienId = nv.Id
    WHERE (@KhoaId IS NULL OR nv.KhoaId = @KhoaId)
        AND (@TuNgay IS NULL OR dv.Ngay >= @TuNgay)
        AND (@DenNgay IS NULL OR dv.Ngay <= @DenNgay)
    GROUP BY LoaiDichVu ORDER BY TongChiPhi DESC;
END
')
GO

PRINT 'Database initialization completed successfully!'
GO

/****** Object:  StoredProcedure [dbo].[sp_CheckNguoiDungExists]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_CheckNguoiDungExists]
    @TenDangNhap NVARCHAR(100),
    @Exists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM NguoiDung WHERE TenDangNhap = @TenDangNhap)
        SET @Exists = 1;
    ELSE
        SET @Exists = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateDoctor]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_CreateDoctor]
    @HoTen NVARCHAR(255),
    @ChuyenKhoa NVARCHAR(255) = NULL,
    @ThongTinLienHe NVARCHAR(255),
    @KhoaId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NewId UNIQUEIDENTIFIER = NEWID()
    
    -- Tự động lấy TenKhoa nếu có KhoaId
    IF @KhoaId IS NOT NULL
    BEGIN
        SELECT @ChuyenKhoa = TenKhoa
        FROM KhoaPhong
        WHERE Id = @KhoaId
    END
    
    INSERT INTO BacSi (Id, HoTen, ChuyenKhoa, ThongTinLienHe, KhoaId)
    VALUES (@NewId, @HoTen, @ChuyenKhoa, @ThongTinLienHe, @KhoaId)
    
    -- Trả về bản ghi vừa tạo
    SELECT 
        b.Id, 
        b.HoTen, 
        b.ChuyenKhoa, 
        b.ThongTinLienHe,
        b.KhoaId,
        k.TenKhoa
    FROM BacSi b
    LEFT JOIN KhoaPhong k ON b.KhoaId = k.Id
    WHERE b.Id = @NewId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateLabTest]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_CreateLabTest]
    @NhapVienId UNIQUEIDENTIFIER = NULL,
    @BacSiId UNIQUEIDENTIFIER = NULL,
    @LoaiXetNghiem NVARCHAR(255) = NULL,
    @KetQua NVARCHAR(MAX) = NULL,
    @Ngay DATETIME2 = NULL,
    @NguoiDungId UNIQUEIDENTIFIER = NULL,   -- User đang đăng nhập
    @AuditUser NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NewId UNIQUEIDENTIFIER = NEWID();
    DECLARE @Now DATETIME2 = GETDATE();
    
    IF @Ngay IS NULL SET @Ngay = @Now;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        INSERT INTO XetNghiem (Id, NhapVienId, BacSiId, LoaiXetNghiem, KetQua, Ngay)
        VALUES (@NewId, @NhapVienId, @BacSiId, @LoaiXetNghiem, @KetQua, @Ngay);
        
        -- Ghi Nhật ký hệ thống
        INSERT INTO NhatKyHeThong (Id, NguoiDungId, HanhDong, ThoiGian, MoTa)
        VALUES (
            NEWID(),
            @NguoiDungId,
            N'Tạo xét nghiệm',
            @Now,
            N'Tạo xét nghiệm: ' + ISNULL(@LoaiXetNghiem, '') + N' bởi ' + ISNULL(@AuditUser, 'System')
        );
        
        SELECT Id, NhapVienId, BacSiId, LoaiXetNghiem, KetQua, Ngay
        FROM XetNghiem WHERE Id = @NewId;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateMedicalRecord]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_CreateMedicalRecord]
    @NhapVienId UNIQUEIDENTIFIER,
    @TienSuBenh NVARCHAR(MAX) = NULL,
    @ChanDoanBanDau NVARCHAR(MAX) = NULL,
    @PhuongAnDieuTri NVARCHAR(MAX) = NULL,
    @KetQuaDieuTri NVARCHAR(255) = NULL,
    @ChanDoanRaVien NVARCHAR(MAX) = NULL,
    @BacSiPhuTrachId UNIQUEIDENTIFIER,      -- Bác sĩ phụ trách (FK BacSi)
    @NguoiDungId UNIQUEIDENTIFIER = NULL,   -- User đang đăng nhập (FK NguoiDung)
    @AuditUser NVARCHAR(255) = NULL          -- Tên user đăng nhập
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NewId UNIQUEIDENTIFIER = NEWID();
    DECLARE @Now DATETIME = GETDATE();
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        INSERT INTO HoSoBenhAn (
            Id, NhapVienId, TienSuBenh, ChanDoanBanDau, ChanDoanRaVien,
            PhuongAnDieuTri, KetQuaDieuTri, NgayLap, BacSiPhuTrachId
        )
        VALUES (
            @NewId, @NhapVienId, @TienSuBenh, @ChanDoanBanDau, @ChanDoanRaVien,
            @PhuongAnDieuTri, @KetQuaDieuTri, @Now, @BacSiPhuTrachId
        );
        
        -- Ghi Nhật ký hệ thống với NguoiDungId từ JWT
        INSERT INTO NhatKyHeThong (Id, NguoiDungId, HanhDong, ThoiGian, MoTa)
        VALUES (
            NEWID(),
            @NguoiDungId,  -- NULL nếu user không tồn tại trong NguoiDung
            N'Tạo hồ sơ bệnh án',
            @Now,
            N'Tạo HSBA cho Nhập viện ' + CAST(@NhapVienId AS NVARCHAR(50))
            + N' bởi ' + ISNULL(@AuditUser, N'System')
        );
        
        -- Return với JOIN
        SELECT 
            hs.Id, hs.NhapVienId, hs.TienSuBenh, hs.ChanDoanBanDau,
            hs.PhuongAnDieuTri, hs.KetQuaDieuTri, hs.ChanDoanRaVien,
            hs.NgayLap, hs.BacSiPhuTrachId,
            bn.HoTen AS TenBenhNhan,
            bn.NgaySinh AS NgaySinhBenhNhan,
            bn.Id AS BenhNhanId
        FROM HoSoBenhAn hs
        LEFT JOIN NhapVien nv ON hs.NhapVienId = nv.Id
        LEFT JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
        WHERE hs.Id = @NewId;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateNguoiDung]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_CreateNguoiDung]
    @Id UNIQUEIDENTIFIER,
    @TenDangNhap NVARCHAR(100),
    @MatKhauHash NVARCHAR(256),
    @VaiTro NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO NguoiDung (Id, TenDangNhap, MatKhauHash, VaiTro)
    VALUES (@Id, @TenDangNhap, @MatKhauHash, @VaiTro);
    
    SELECT @Id AS Id, @TenDangNhap AS TenDangNhap, @VaiTro AS VaiTro;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateNguoiDungFull]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_CreateNguoiDungFull]
    @Id UNIQUEIDENTIFIER,
    @TenDangNhap NVARCHAR(100),
    @MatKhauHash NVARCHAR(256),
    @VaiTro NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO NguoiDung (Id, TenDangNhap, MatKhauHash, VaiTro)
    OUTPUT INSERTED.Id, INSERTED.TenDangNhap, INSERTED.MatKhauHash, INSERTED.VaiTro
    VALUES (@Id, @TenDangNhap, @MatKhauHash, @VaiTro);
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateSurgery]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- 7. sp_CreateSurgery
-- =============================================
CREATE PROCEDURE [dbo].[sp_CreateSurgery]
    @NhapVienId UNIQUEIDENTIFIER = NULL,
    @BacSiChinhId UNIQUEIDENTIFIER = NULL,
    @LoaiPhauThuat NVARCHAR(255) = NULL,
    @Ekip NVARCHAR(MAX) = NULL,
    @Ngay DATETIME2 = NULL,
    @PhongMo NVARCHAR(100) = NULL,
    @TrangThai NVARCHAR(50) = NULL,
    @NguoiDungId UNIQUEIDENTIFIER = NULL,
    @AuditUser NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NewId UNIQUEIDENTIFIER = NEWID();
    DECLARE @Now DATETIME2 = GETDATE();
    
    IF @Ngay IS NULL SET @Ngay = @Now;
    IF @TrangThai IS NULL SET @TrangThai = N'Chờ thực hiện';
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        INSERT INTO PhauThuat (Id, NhapVienId, BacSiChinhId, LoaiPhauThuat, Ekip, Ngay, PhongMo, TrangThai)
        VALUES (@NewId, @NhapVienId, @BacSiChinhId, @LoaiPhauThuat, @Ekip, @Ngay, @PhongMo, @TrangThai);
        
        INSERT INTO NhatKyHeThong (Id, NguoiDungId, HanhDong, ThoiGian, MoTa)
        VALUES (
            NEWID(),
            @NguoiDungId,
            N'Lên lịch phẫu thuật',
            @Now,
            N'Lên lịch PT: ' + ISNULL(@LoaiPhauThuat, '') + N' bởi ' + ISNULL(@AuditUser, 'System')
        );
        
        SELECT Id, NhapVienId, BacSiChinhId, LoaiPhauThuat, Ekip, Ngay, PhongMo, TrangThai
        FROM PhauThuat WHERE Id = @NewId;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteDoctor]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DeleteDoctor]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM BacSi
    WHERE Id = @Id
    
    -- Trả về số rows bị xóa
    RETURN @@ROWCOUNT
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteLabTest]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- 6. sp_DeleteLabTest
-- =============================================
CREATE PROCEDURE [dbo].[sp_DeleteLabTest]
    @Id UNIQUEIDENTIFIER,
    @NguoiDungId UNIQUEIDENTIFIER = NULL,
    @AuditUser NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = GETDATE();
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DELETE FROM XetNghiem WHERE Id = @Id;
        
        INSERT INTO NhatKyHeThong (Id, NguoiDungId, HanhDong, ThoiGian, MoTa)
        VALUES (
            NEWID(),
            @NguoiDungId,
            N'Xóa xét nghiệm',
            @Now,
            N'Xóa xét nghiệm ID: ' + CAST(@Id AS NVARCHAR(50)) + N' bởi ' + ISNULL(@AuditUser, 'System')
        );
        
        COMMIT TRANSACTION;
        RETURN @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteMedicalRecord]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- sp_DeleteMedicalRecord
-- =============================================
CREATE PROCEDURE [dbo].[sp_DeleteMedicalRecord]
    @Id UNIQUEIDENTIFIER,
    @NguoiDungId UNIQUEIDENTIFIER = NULL,
    @AuditUser NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME = GETDATE();
    DECLARE @OldChanDoan NVARCHAR(MAX);
    DECLARE @OldKetQua NVARCHAR(255);

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Lấy giá trị cũ
        SELECT @OldChanDoan = ChanDoanBanDau, @OldKetQua = KetQuaDieuTri
        FROM HoSoBenhAn WHERE Id = @Id;

        -- Ghi Audit trước khi xóa (THÊM NguoiDungId)
        INSERT INTO Audit_HoSoBenhAn (Id, HoSoBenhAnId, HanhDong, ChanDoanCu, KetQuaCu, NguoiDungId, NguoiSua, ThoiGianSua)
        VALUES (NEWID(), @Id, N'DELETE', @OldChanDoan, @OldKetQua, @NguoiDungId, @AuditUser, @Now);

        -- Xóa dữ liệu
        DELETE FROM HoSoBenhAn WHERE Id = @Id;

        -- Ghi Nhật ký hệ thống
        INSERT INTO NhatKyHeThong (Id, NguoiDungId, HanhDong, ThoiGian, MoTa)
        VALUES (
            NEWID(), @NguoiDungId, N'Xóa hồ sơ bệnh án', @Now,
            N'Xóa HSBA ' + CAST(@Id AS NVARCHAR(50)) + N' bởi ' + ISNULL(@AuditUser, N'System')
        );

        COMMIT TRANSACTION;
        RETURN @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteNguoiDung]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DeleteNguoiDung]
    @Id UNIQUEIDENTIFIER,
    @RowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM NguoiDung WHERE Id = @Id;
    SET @RowsAffected = @@ROWCOUNT;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteSurgery]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- 9. sp_DeleteSurgery
-- =============================================
CREATE PROCEDURE [dbo].[sp_DeleteSurgery]
    @Id UNIQUEIDENTIFIER,
    @NguoiDungId UNIQUEIDENTIFIER = NULL,
    @AuditUser NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = GETDATE();
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DELETE FROM PhauThuat WHERE Id = @Id;
        
        INSERT INTO NhatKyHeThong (Id, NguoiDungId, HanhDong, ThoiGian, MoTa)
        VALUES (
            NEWID(),
            @NguoiDungId,
            N'Hủy lịch phẫu thuật',
            @Now,
            N'Hủy PT ID: ' + CAST(@Id AS NVARCHAR(50)) + N' bởi ' + ISNULL(@AuditUser, 'System')
        );
        
        COMMIT TRANSACTION;
        RETURN @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllDoctors]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetAllDoctors]
AS
BEGIN 
    SELECT 
        b.Id, 
        b.HoTen,
        b.ChuyenKhoa,
        b.ThongTinLienHe,
        b.KhoaId,
        k.TenKhoa
    FROM [dbo].[BacSi] b
    LEFT JOIN [dbo].[KhoaPhong] k ON b.KhoaId = k.Id
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllLabTests]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetAllLabTests]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        xn.Id, 
        xn.NhapVienId, 
        xn.BacSiId, 
        xn.LoaiXetNghiem, 
        xn.KetQua, 
        xn.Ngay,
        xn.DonGia,
        bn.HoTen AS TenBenhNhan,
        bn.NgaySinh AS NgaySinhBenhNhan,
        bn.Id AS BenhNhanId
    FROM XetNghiem xn
    INNER JOIN NhapVien nv ON xn.NhapVienId = nv.Id
    INNER JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    ORDER BY xn.Ngay DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllMedicalRecords]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetAllMedicalRecords]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT
        hs.Id,
        hs.NhapVienId,
        hs.TienSuBenh,
        hs.ChanDoanBanDau,
        hs.ChanDoanRaVien,
        hs.PhuongAnDieuTri,
        hs.KetQuaDieuTri,
        hs.NgayLap,
        hs.BacSiPhuTrachId,
        bn.HoTen AS TenBenhNhan,
        bn.NgaySinh AS NgaySinhBenhNhan,
        bn.Id AS BenhNhanId
    FROM HoSoBenhAn hs
    INNER JOIN NhapVien nv ON hs.NhapVienId = nv.Id
    INNER JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    ORDER BY hs.NgayLap DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllNguoiDung]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAllNguoiDung]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, TenDangNhap, VaiTro 
    FROM NguoiDung 
    ORDER BY TenDangNhap;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllNguoiDungFull]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAllNguoiDungFull]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, TenDangNhap, MatKhauHash, VaiTro 
    FROM NguoiDung;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllSurgeries]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetAllSurgeries]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        pt.Id,
        pt.NhapVienId,
        pt.BacSiChinhId,
        pt.LoaiPhauThuat,
        pt.Ekip,
        pt.Ngay,
        pt.PhongMo,
        pt.TrangThai,
        bn.HoTen AS TenBenhNhan,
        bn.NgaySinh AS NgaySinhBenhNhan,
        bn.Id AS BenhNhanId
    FROM PhauThuat pt
    INNER JOIN NhapVien nv ON pt.NhapVienId = nv.Id
    INNER JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    ORDER BY pt.Ngay DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAuditHoSoBenhAn]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAuditHoSoBenhAn]
    @NguoiDungId UNIQUEIDENTIFIER = NULL,
    @HanhDong NVARCHAR(200) = NULL,
    @TuNgay DATETIME = NULL,
    @DenNgay DATETIME = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Count total records
    SELECT @TotalRecords = COUNT(*) 
    FROM Audit_HoSoBenhAn a
    LEFT JOIN NguoiDung nd ON a.NguoiDungId = nd.Id
    WHERE (@NguoiDungId IS NULL OR a.NguoiDungId = @NguoiDungId)
      AND (@HanhDong IS NULL OR a.HanhDong LIKE '%' + @HanhDong + '%')
      AND (@TuNgay IS NULL OR a.ThoiGianSua >= @TuNgay)
      AND (@DenNgay IS NULL OR a.ThoiGianSua <= @DenNgay);
    
    -- Get paged data
    SELECT a.Id, a.HoSoBenhAnId, a.HanhDong, a.ChanDoanCu, a.KetQuaCu, 
           a.NguoiDungId, COALESCE(nd.TenDangNhap, a.NguoiSua) AS TenNguoiSua, a.ThoiGianSua
    FROM Audit_HoSoBenhAn a
    LEFT JOIN NguoiDung nd ON a.NguoiDungId = nd.Id
    WHERE (@NguoiDungId IS NULL OR a.NguoiDungId = @NguoiDungId)
      AND (@HanhDong IS NULL OR a.HanhDong LIKE '%' + @HanhDong + '%')
      AND (@TuNgay IS NULL OR a.ThoiGianSua >= @TuNgay)
      AND (@DenNgay IS NULL OR a.ThoiGianSua <= @DenNgay)
    ORDER BY a.ThoiGianSua DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS 
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetBedCapacityReport]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetBedCapacityReport]
    @KhoaId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        kp.Id AS KhoaId,
        kp.TenKhoa,
        COUNT(gb.Id) AS TongGiuong,
        SUM(CASE WHEN gb.TrangThai = N'Đang sử dụng' OR gb.TrangThai = 'DangSuDung' THEN 1 ELSE 0 END) AS GiuongDangSuDung,
        SUM(CASE WHEN gb.TrangThai = N'Trống' OR gb.TrangThai = 'Trong' OR gb.TrangThai IS NULL THEN 1 ELSE 0 END) AS GiuongTrong
    FROM KhoaPhong kp
    LEFT JOIN GiuongBenh gb ON kp.Id = gb.KhoaId
    WHERE (@KhoaId IS NULL OR kp.Id = @KhoaId)
    GROUP BY kp.Id, kp.TenKhoa
    ORDER BY kp.TenKhoa;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetDoctorById]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetDoctorById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SELECT 
        b.Id, 
        b.HoTen, 
        b.ChuyenKhoa, 
        b.ThongTinLienHe,
        b.KhoaId,
        k.TenKhoa
    FROM BacSi b
    LEFT JOIN KhoaPhong k ON b.KhoaId = k.Id
    WHERE b.Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetMedicalRecords]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetMedicalRecords]
    @NhapVienId UNIQUEIDENTIFIER = NULL,
    @SearchTerm NVARCHAR(255) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Tổng bản ghi
    SELECT @TotalRecords = COUNT(*)
    FROM HoSoBenhAn hs
    INNER JOIN NhapVien nv ON hs.NhapVienId = nv.Id
    INNER JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    WHERE (@NhapVienId IS NULL OR hs.NhapVienId = @NhapVienId)
      AND (
            @SearchTerm IS NULL
            OR hs.TienSuBenh LIKE N'%' + @SearchTerm + '%'
            OR hs.ChanDoanBanDau LIKE N'%' + @SearchTerm + '%'
            OR hs.ChanDoanRaVien LIKE N'%' + @SearchTerm + '%'
            OR bn.HoTen LIKE N'%' + @SearchTerm + '%'
          );
    
    -- Phân trang với thông tin bệnh nhân
    SELECT
        hs.Id,
        hs.NhapVienId,
        hs.TienSuBenh,
        hs.ChanDoanBanDau,
        hs.ChanDoanRaVien,
        hs.PhuongAnDieuTri,
        hs.KetQuaDieuTri,
        hs.NgayLap,
        hs.BacSiPhuTrachId,
        bn.HoTen AS TenBenhNhan,
        bn.NgaySinh AS NgaySinhBenhNhan,
        bn.Id AS BenhNhanId
    FROM HoSoBenhAn hs
    INNER JOIN NhapVien nv ON hs.NhapVienId = nv.Id
    INNER JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    WHERE (@NhapVienId IS NULL OR hs.NhapVienId = @NhapVienId)
      AND (
            @SearchTerm IS NULL
            OR hs.TienSuBenh LIKE N'%' + @SearchTerm + '%'
            OR hs.ChanDoanBanDau LIKE N'%' + @SearchTerm + '%'
            OR hs.ChanDoanRaVien LIKE N'%' + @SearchTerm + '%'
            OR bn.HoTen LIKE N'%' + @SearchTerm + '%'
          )
    ORDER BY hs.NgayLap DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetNguoiDungById]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetNguoiDungById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, TenDangNhap, VaiTro 
    FROM NguoiDung 
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetNguoiDungByIdFull]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetNguoiDungByIdFull]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, TenDangNhap, MatKhauHash, VaiTro 
    FROM NguoiDung 
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetNguoiDungByTenDangNhap]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetNguoiDungByTenDangNhap]
    @TenDangNhap NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, TenDangNhap, VaiTro 
    FROM NguoiDung 
    WHERE TenDangNhap = @TenDangNhap;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetNguoiDungByTenDangNhapFull]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetNguoiDungByTenDangNhapFull]
    @TenDangNhap NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, TenDangNhap, MatKhauHash, VaiTro 
    FROM NguoiDung 
    WHERE TenDangNhap = @TenDangNhap;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetNhatKyHeThong]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetNhatKyHeThong]
    @NguoiDungId UNIQUEIDENTIFIER = NULL,
    @HanhDong NVARCHAR(200) = NULL,
    @TuNgay DATETIME = NULL,
    @DenNgay DATETIME = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Count total records
    SELECT @TotalRecords = COUNT(*) 
    FROM NhatKyHeThong nk
    LEFT JOIN NguoiDung nd ON nk.NguoiDungId = nd.Id
    WHERE (@NguoiDungId IS NULL OR nk.NguoiDungId = @NguoiDungId)
      AND (@HanhDong IS NULL OR nk.HanhDong LIKE '%' + @HanhDong + '%')
      AND (@TuNgay IS NULL OR nk.ThoiGian >= @TuNgay)
      AND (@DenNgay IS NULL OR nk.ThoiGian <= @DenNgay);
    
    -- Get paged data
    SELECT nk.Id, nk.NguoiDungId, nd.TenDangNhap AS TenNguoiDung, 
           nk.HanhDong, nk.ThoiGian, nk.MoTa
    FROM NhatKyHeThong nk
    LEFT JOIN NguoiDung nd ON nk.NguoiDungId = nd.Id
    WHERE (@NguoiDungId IS NULL OR nk.NguoiDungId = @NguoiDungId)
      AND (@HanhDong IS NULL OR nk.HanhDong LIKE '%' + @HanhDong + '%')
      AND (@TuNgay IS NULL OR nk.ThoiGian >= @TuNgay)
      AND (@DenNgay IS NULL OR nk.ThoiGian <= @DenNgay)
    ORDER BY nk.ThoiGian DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS 
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTreatmentCostByDepartment]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_GetTreatmentCostByDepartment]
    @KhoaId UNIQUEIDENTIFIER = NULL,
    @TuNgay DATETIME = NULL,
    @DenNgay DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        kp.Id AS KhoaId,
        kp.TenKhoa,
        ISNULL(SUM(dv.DonGia * dv.SoLuong), 0) AS TongChiPhiDichVu,
        ISNULL(SUM(pt.ChiPhi), 0) AS TongChiPhiPhauThuat,
        ISNULL(SUM(xn.DonGia), 0) AS TongChiPhiXetNghiem,
        COUNT(DISTINCT nv.Id) AS SoLuotDieuTri
    FROM KhoaPhong kp
    LEFT JOIN NhapVien nv ON kp.Id = nv.KhoaId
        AND (@TuNgay IS NULL OR nv.NgayNhap >= @TuNgay)
        AND (@DenNgay IS NULL OR nv.NgayNhap <= @DenNgay)
    LEFT JOIN DichVuDieuTri dv ON nv.Id = dv.NhapVienId
    LEFT JOIN PhauThuat pt ON nv.Id = pt.NhapVienId
    LEFT JOIN XetNghiem xn ON nv.Id = xn.NhapVienId
    WHERE (@KhoaId IS NULL OR kp.Id = @KhoaId)
    GROUP BY kp.Id, kp.TenKhoa
    ORDER BY kp.TenKhoa;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTreatmentCostByServiceType]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_GetTreatmentCostByServiceType]
    @KhoaId UNIQUEIDENTIFIER = NULL,
    @TuNgay DATETIME = NULL,
    @DenNgay DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        LoaiDichVu,
        SUM(ISNULL(DonGia, 0) * ISNULL(SoLuong, 1)) AS TongChiPhi,
        SUM(ISNULL(SoLuong, 1)) AS SoLuong
    FROM DichVuDieuTri dv
    INNER JOIN NhapVien nv ON dv.NhapVienId = nv.Id
    WHERE (@KhoaId IS NULL OR nv.KhoaId = @KhoaId)
        AND (@TuNgay IS NULL OR dv.Ngay >= @TuNgay)
        AND (@DenNgay IS NULL OR dv.Ngay <= @DenNgay)
    GROUP BY LoaiDichVu
    ORDER BY TongChiPhi DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_LookupPatients]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_LookupPatients]
    @Term NVARCHAR(255) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Đếm tổng số bản ghi
    SELECT @TotalRecords = COUNT(*)
    FROM BenhNhan bn
    WHERE @Term IS NULL 
       OR bn.HoTen LIKE N'%' + @Term + '%'
       OR bn.SoTheBaoHiem LIKE N'%' + @Term + '%'
       OR bn.DiaChi LIKE N'%' + @Term + '%';
    
    -- Lấy dữ liệu phân trang với thông tin nhập viện gần nhất
    SELECT 
        bn.Id,
        bn.HoTen,
        bn.SoTheBaoHiem,
        bn.DiaChi,
        bn.GioiTinh,
        nv.Id AS NhapVienId,
        nv.NgayNhap,
        nv.NgayXuat,
        nv.TrangThai AS TrangThaiNhapVien,
        nv.KhoaId,
        kp.TenKhoa
    FROM BenhNhan bn
    -- Lấy lần nhập viện gần nhất
    OUTER APPLY (
        SELECT TOP 1 
            nv.Id,
            nv.NgayNhap,
            nv.NgayXuat,
            nv.TrangThai,
            nv.KhoaId
        FROM NhapVien nv
        WHERE nv.BenhNhanId = bn.Id
        ORDER BY nv.NgayNhap DESC
    ) nv
    -- Join với bảng KhoaPhong
    LEFT JOIN KhoaPhong kp ON nv.KhoaId = kp.Id
    WHERE @Term IS NULL 
       OR bn.HoTen LIKE N'%' + @Term + '%'
       OR bn.SoTheBaoHiem LIKE N'%' + @Term + '%'
       OR bn.DiaChi LIKE N'%' + @Term + '%'
    ORDER BY bn.HoTen
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ResetPassword]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ResetPassword]
    @Id UNIQUEIDENTIFIER,
    @MatKhauHash NVARCHAR(256),
    @RowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE NguoiDung 
    SET MatKhauHash = @MatKhauHash 
    WHERE Id = @Id;
    
    SET @RowsAffected = @@ROWCOUNT;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SearchDoctors]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_SearchDoctors]
    @SearchTerm NVARCHAR(255) = NULL,
    @HoTen NVARCHAR(255) = NULL,
    @ChuyenKhoa NVARCHAR(255) = NULL,
    @ThongTinLienHe NVARCHAR(255) = NULL,
    @KhoaId UNIQUEIDENTIFIER = NULL, -- Thêm tham số mới
    @SortBy NVARCHAR(50) = 'HoTen',
    @SortOrder NVARCHAR(4) = 'ASC',
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Tính tổng số bản ghi
    SELECT @TotalRecords = COUNT(*)
    FROM BacSi b
    LEFT JOIN KhoaPhong k ON b.KhoaId = k.Id
    WHERE (@SearchTerm IS NULL OR b.HoTen LIKE N'%' + @SearchTerm + '%' OR k.TenKhoa LIKE N'%' + @SearchTerm + '%')
      AND (@HoTen IS NULL OR b.HoTen = @HoTen)
      AND (@ChuyenKhoa IS NULL OR b.ChuyenKhoa = @ChuyenKhoa)
      AND (@ThongTinLienHe IS NULL OR b.ThongTinLienHe LIKE N'%' + @ThongTinLienHe + '%')
      AND (@KhoaId IS NULL OR b.KhoaId = @KhoaId)
    
    -- Truy vấn dữ liệu với phân trang
    DECLARE @SQL NVARCHAR(MAX)
    SET @SQL = N'
    SELECT b.Id, b.HoTen, b.ChuyenKhoa, b.ThongTinLienHe, b.KhoaId, k.TenKhoa
    FROM BacSi b
    LEFT JOIN KhoaPhong k ON b.KhoaId = k.Id
    WHERE (@SearchTerm IS NULL OR b.HoTen LIKE N''%'' + @SearchTerm + ''%'' OR k.TenKhoa LIKE N''%'' + @SearchTerm + ''%'')
      AND (@HoTen IS NULL OR b.HoTen = @HoTen)
      AND (@ChuyenKhoa IS NULL OR b.ChuyenKhoa = @ChuyenKhoa)
      AND (@ThongTinLienHe IS NULL OR b.ThongTinLienHe LIKE N''%'' + @ThongTinLienHe + ''%'')
      AND (@KhoaId IS NULL OR b.KhoaId = @KhoaId)
    ORDER BY '
    
    -- Thêm điều kiện sắp xếp
    IF @SortBy = 'ChuyenKhoa'
        SET @SQL = @SQL + N'b.ChuyenKhoa '
    ELSE IF @SortBy = 'TenKhoa'
        SET @SQL = @SQL + N'k.TenKhoa '
    ELSE
        SET @SQL = @SQL + N'b.HoTen '
    
    -- Thêm chiều sắp xếp
    IF UPPER(@SortOrder) = 'DESC'
        SET @SQL = @SQL + N'DESC '
    ELSE
        SET @SQL = @SQL + N'ASC '
    
    -- Phân trang
    SET @SQL = @SQL + N'
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY'
    
    -- Thực thi
    EXEC sp_executesql @SQL,
        N'@SearchTerm NVARCHAR(255), @HoTen NVARCHAR(255), @ChuyenKhoa NVARCHAR(255), @ThongTinLienHe NVARCHAR(255), @KhoaId UNIQUEIDENTIFIER, @PageNumber INT, @PageSize INT',
        @SearchTerm, @HoTen, @ChuyenKhoa, @ThongTinLienHe, @KhoaId, @PageNumber, @PageSize
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SearchLabTests]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_SearchLabTests]
    @NhapVienId UNIQUEIDENTIFIER = NULL,
    @SearchTerm NVARCHAR(255) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Đếm tổng số bản ghi
    SELECT @TotalRecords = COUNT(*)
    FROM XetNghiem xn
    INNER JOIN NhapVien nv ON xn.NhapVienId = nv.Id
    INNER JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    WHERE (@NhapVienId IS NULL OR xn.NhapVienId = @NhapVienId)
      AND (
            @SearchTerm IS NULL 
            OR xn.LoaiXetNghiem LIKE N'%' + @SearchTerm + '%' 
            OR xn.KetQua LIKE N'%' + @SearchTerm + '%'
            OR bn.HoTen LIKE N'%' + @SearchTerm + '%'
          );
    
    -- Lấy dữ liệu phân trang
    SELECT 
        xn.Id, 
        xn.NhapVienId, 
        xn.BacSiId, 
        xn.LoaiXetNghiem, 
        xn.KetQua, 
        xn.Ngay,
        xn.DonGia,
        bn.HoTen AS TenBenhNhan,
        bn.NgaySinh AS NgaySinhBenhNhan,
        bn.Id AS BenhNhanId
    FROM XetNghiem xn
    INNER JOIN NhapVien nv ON xn.NhapVienId = nv.Id
    INNER JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    WHERE (@NhapVienId IS NULL OR xn.NhapVienId = @NhapVienId)
      AND (
            @SearchTerm IS NULL 
            OR xn.LoaiXetNghiem LIKE N'%' + @SearchTerm + '%' 
            OR xn.KetQua LIKE N'%' + @SearchTerm + '%'
            OR bn.HoTen LIKE N'%' + @SearchTerm + '%'
          )
    ORDER BY xn.Ngay DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SearchNguoiDung]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SearchNguoiDung]
    @TenDangNhap NVARCHAR(100) = NULL,
    @VaiTro NVARCHAR(50) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Count total
    SELECT @TotalRecords = COUNT(*) 
    FROM NguoiDung
    WHERE (@TenDangNhap IS NULL OR TenDangNhap LIKE '%' + @TenDangNhap + '%')
      AND (@VaiTro IS NULL OR VaiTro = @VaiTro);
    
    -- Get paged data
    SELECT Id, TenDangNhap, VaiTro 
    FROM NguoiDung
    WHERE (@TenDangNhap IS NULL OR TenDangNhap LIKE '%' + @TenDangNhap + '%')
      AND (@VaiTro IS NULL OR VaiTro = @VaiTro)
    ORDER BY TenDangNhap
    OFFSET (@PageNumber - 1) * @PageSize ROWS 
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SearchSurgeries]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_SearchSurgeries]
    @BacSiId UNIQUEIDENTIFIER = NULL,
    @SearchTerm NVARCHAR(255) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Đếm tổng số bản ghi
    SELECT @TotalRecords = COUNT(*)
    FROM PhauThuat pt
    INNER JOIN NhapVien nv ON pt.NhapVienId = nv.Id
    INNER JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    WHERE (@BacSiId IS NULL OR pt.BacSiChinhId = @BacSiId)
      AND (
            @SearchTerm IS NULL 
            OR pt.LoaiPhauThuat LIKE N'%' + @SearchTerm + '%' 
            OR pt.PhongMo LIKE N'%' + @SearchTerm + '%'
            OR bn.HoTen LIKE N'%' + @SearchTerm + '%'
          );
    
    -- Lấy dữ liệu phân trang
    SELECT 
        pt.Id,
        pt.NhapVienId,
        pt.BacSiChinhId,
        pt.LoaiPhauThuat,
        pt.Ekip,
        pt.Ngay,
        pt.PhongMo,
        pt.TrangThai,
        bn.HoTen AS TenBenhNhan,
        bn.NgaySinh AS NgaySinhBenhNhan,
        bn.Id AS BenhNhanId
    FROM PhauThuat pt
    INNER JOIN NhapVien nv ON pt.NhapVienId = nv.Id
    INNER JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
    WHERE (@BacSiId IS NULL OR pt.BacSiChinhId = @BacSiId)
      AND (
            @SearchTerm IS NULL 
            OR pt.LoaiPhauThuat LIKE N'%' + @SearchTerm + '%' 
            OR pt.PhongMo LIKE N'%' + @SearchTerm + '%'
            OR bn.HoTen LIKE N'%' + @SearchTerm + '%'
          )
    ORDER BY pt.Ngay DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateDoctor]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_UpdateDoctor]
    @Id UNIQUEIDENTIFIER,
    @HoTen NVARCHAR(255),
    @ChuyenKhoa NVARCHAR(255) = NULL,
    @ThongTinLienHe NVARCHAR(255),
    @KhoaId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Tự động lấy TenKhoa nếu có KhoaId
    IF @KhoaId IS NOT NULL
    BEGIN
        SELECT @ChuyenKhoa = TenKhoa
        FROM KhoaPhong
        WHERE Id = @KhoaId
    END
    
    UPDATE BacSi
    SET HoTen = @HoTen,
        ChuyenKhoa = @ChuyenKhoa,
        ThongTinLienHe = @ThongTinLienHe,
        KhoaId = @KhoaId
    WHERE Id = @Id
    
    -- Trả về bản ghi đã update
    SELECT 
        b.Id, 
        b.HoTen, 
        b.ChuyenKhoa, 
        b.ThongTinLienHe,
        b.KhoaId,
        k.TenKhoa
    FROM BacSi b
    LEFT JOIN KhoaPhong k ON b.KhoaId = k.Id
    WHERE b.Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateLabTest]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- 5. sp_UpdateLabTest
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateLabTest]
    @Id UNIQUEIDENTIFIER,
    @LoaiXetNghiem NVARCHAR(255) = NULL,
    @KetQua NVARCHAR(MAX) = NULL,
    @Ngay DATETIME2 = NULL,
    @NguoiDungId UNIQUEIDENTIFIER = NULL,
    @AuditUser NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = GETDATE();
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE XetNghiem
        SET LoaiXetNghiem = ISNULL(@LoaiXetNghiem, LoaiXetNghiem),
            KetQua = ISNULL(@KetQua, KetQua),
            Ngay = ISNULL(@Ngay, Ngay)
        WHERE Id = @Id;
        
        INSERT INTO NhatKyHeThong (Id, NguoiDungId, HanhDong, ThoiGian, MoTa)
        VALUES (
            NEWID(),
            @NguoiDungId,
            N'Cập nhật xét nghiệm',
            @Now,
            N'Cập nhật xét nghiệm ID: ' + CAST(@Id AS NVARCHAR(50)) + N' bởi ' + ISNULL(@AuditUser, 'System')
        );
        
        SELECT Id, NhapVienId, BacSiId, LoaiXetNghiem, KetQua, Ngay
        FROM XetNghiem WHERE Id = @Id;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateMedicalRecord]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- sp_UpdateMedicalRecord
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateMedicalRecord]
    @Id UNIQUEIDENTIFIER,
    @TienSuBenh NVARCHAR(MAX) = NULL,
    @ChanDoanBanDau NVARCHAR(MAX) = NULL,
    @ChanDoanRaVien NVARCHAR(MAX) = NULL,
    @PhuongAnDieuTri NVARCHAR(MAX) = NULL,
    @KetQuaDieuTri NVARCHAR(255) = NULL,
    @BacSiPhuTrachId UNIQUEIDENTIFIER = NULL,
    @NguoiDungId UNIQUEIDENTIFIER = NULL,
    @AuditUser NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME = GETDATE();
    DECLARE @OldChanDoan NVARCHAR(MAX);
    DECLARE @OldKetQua NVARCHAR(255);

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Lấy giá trị cũ
        SELECT @OldChanDoan = ChanDoanBanDau, @OldKetQua = KetQuaDieuTri
        FROM HoSoBenhAn WHERE Id = @Id;

        -- Cập nhật dữ liệu
        UPDATE HoSoBenhAn
        SET
            TienSuBenh = ISNULL(@TienSuBenh, TienSuBenh),
            ChanDoanBanDau = ISNULL(@ChanDoanBanDau, ChanDoanBanDau),
            ChanDoanRaVien = ISNULL(@ChanDoanRaVien, ChanDoanRaVien),
            PhuongAnDieuTri = ISNULL(@PhuongAnDieuTri, PhuongAnDieuTri),
            KetQuaDieuTri = ISNULL(@KetQuaDieuTri, KetQuaDieuTri)
        WHERE Id = @Id;

        -- Ghi Audit bệnh án (THÊM NguoiDungId)
        INSERT INTO Audit_HoSoBenhAn (Id, HoSoBenhAnId, HanhDong, ChanDoanCu, KetQuaCu, NguoiDungId, NguoiSua, ThoiGianSua)
        VALUES (NEWID(), @Id, N'UPDATE', @OldChanDoan, @OldKetQua, @NguoiDungId, @AuditUser, @Now);

        -- Ghi Nhật ký hệ thống
        INSERT INTO NhatKyHeThong (Id, NguoiDungId, HanhDong, ThoiGian, MoTa)
        VALUES (
            NEWID(), @NguoiDungId, N'Cập nhật hồ sơ bệnh án', @Now,
            N'Cập nhật HSBA ' + CAST(@Id AS NVARCHAR(50)) + N' bởi ' + ISNULL(@AuditUser, N'System')
        );

        -- Return với JOIN
        SELECT 
            hs.Id, hs.NhapVienId, hs.TienSuBenh, hs.ChanDoanBanDau,
            hs.PhuongAnDieuTri, hs.KetQuaDieuTri, hs.ChanDoanRaVien,
            hs.NgayLap, hs.BacSiPhuTrachId,
            bn.HoTen AS TenBenhNhan,
            bn.NgaySinh AS NgaySinhBenhNhan,
            bn.Id AS BenhNhanId
        FROM HoSoBenhAn hs
        LEFT JOIN NhapVien nv ON hs.NhapVienId = nv.Id
        LEFT JOIN BenhNhan bn ON nv.BenhNhanId = bn.Id
        WHERE hs.Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateNguoiDung]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateNguoiDung]
    @Id UNIQUEIDENTIFIER,
    @TenDangNhap NVARCHAR(100) = NULL,
    @VaiTro NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE NguoiDung 
    SET TenDangNhap = COALESCE(@TenDangNhap, TenDangNhap),
        VaiTro = COALESCE(@VaiTro, VaiTro)
    WHERE Id = @Id;
    
    SELECT Id, TenDangNhap, VaiTro 
    FROM NguoiDung 
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateNguoiDungFull]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateNguoiDungFull]
    @Id UNIQUEIDENTIFIER,
    @TenDangNhap NVARCHAR(100),
    @MatKhauHash NVARCHAR(256),
    @VaiTro NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE NguoiDung 
    SET TenDangNhap = @TenDangNhap, 
        MatKhauHash = @MatKhauHash, 
        VaiTro = @VaiTro
    WHERE Id = @Id;
    
    SELECT Id, TenDangNhap, MatKhauHash, VaiTro 
    FROM NguoiDung 
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateSurgery]    Script Date: 1/8/2026 4:06:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- 8. sp_UpdateSurgery
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateSurgery]
    @Id UNIQUEIDENTIFIER,
    @LoaiPhauThuat NVARCHAR(255) = NULL,
    @Ekip NVARCHAR(MAX) = NULL,
    @Ngay DATETIME2 = NULL,
    @PhongMo NVARCHAR(100) = NULL,
    @TrangThai NVARCHAR(50) = NULL,
    @NguoiDungId UNIQUEIDENTIFIER = NULL,
    @AuditUser NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = GETDATE();
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE PhauThuat
        SET LoaiPhauThuat = ISNULL(@LoaiPhauThuat, LoaiPhauThuat),
            Ekip = ISNULL(@Ekip, Ekip),
            Ngay = ISNULL(@Ngay, Ngay),
            PhongMo = ISNULL(@PhongMo, PhongMo),
            TrangThai = ISNULL(@TrangThai, TrangThai)
        WHERE Id = @Id;
        
        INSERT INTO NhatKyHeThong (Id, NguoiDungId, HanhDong, ThoiGian, MoTa)
        VALUES (
            NEWID(),
            @NguoiDungId,
            N'Cập nhật lịch phẫu thuật',
            @Now,
            N'Cập nhật PT ID: ' + CAST(@Id AS NVARCHAR(50)) + N' bởi ' + ISNULL(@AuditUser, 'System')
        );
        
        SELECT Id, NhapVienId, BacSiChinhId, LoaiPhauThuat, Ekip, Ngay, PhongMo, TrangThai
        FROM PhauThuat WHERE Id = @Id;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
