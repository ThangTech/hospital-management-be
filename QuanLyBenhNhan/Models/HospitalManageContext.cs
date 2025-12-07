using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyBenhNhan.Models;

public partial class HospitalManageContext : DbContext
{
    public HospitalManageContext()
    {
    }

    public HospitalManageContext(DbContextOptions<HospitalManageContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BacSi> BacSis { get; set; }

    public virtual DbSet<BenhNhan> BenhNhans { get; set; }

    public virtual DbSet<DichVuDieuTri> DichVuDieuTris { get; set; }

    public virtual DbSet<DieuDuong> DieuDuongs { get; set; }

    public virtual DbSet<GiuongBenh> GiuongBenhs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhoaPhong> KhoaPhongs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhapVien> NhapViens { get; set; }

    public virtual DbSet<NhatKyHeThong> NhatKyHeThongs { get; set; }

    public virtual DbSet<PhauThuat> PhauThuats { get; set; }

    public virtual DbSet<XetNghiem> XetNghiems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-5J0CNUJ;Database=hospital_manage;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BacSi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BacSi__3214EC071E76A6E3");

            entity.ToTable("BacSi");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ChuyenKhoa).HasMaxLength(255);
            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.ThongTinLienHe).HasMaxLength(255);
        });

        modelBuilder.Entity<BenhNhan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BenhNhan__3214EC07F4E03AD6");

            entity.ToTable("BenhNhan");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.GioiTinh).HasMaxLength(50);
            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.SoTheBaoHiem).HasMaxLength(50);
        });

        modelBuilder.Entity<DichVuDieuTri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DichVuDi__3214EC073E5FAAE2");

            entity.ToTable("DichVuDieuTri");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.LoaiDichVu).HasMaxLength(255);
            entity.Property(e => e.Ngay).HasColumnType("datetime");

            entity.HasOne(d => d.BacSi).WithMany(p => p.DichVuDieuTris)
                .HasForeignKey(d => d.BacSiId)
                .HasConstraintName("FK__DichVuDie__BacSi__4E88ABD4");

            entity.HasOne(d => d.DieuDuong).WithMany(p => p.DichVuDieuTris)
                .HasForeignKey(d => d.DieuDuongId)
                .HasConstraintName("FK__DichVuDie__DieuD__4F7CD00D");

            entity.HasOne(d => d.NhapVien).WithMany(p => p.DichVuDieuTris)
                .HasForeignKey(d => d.NhapVienId)
                .HasConstraintName("FK__DichVuDie__NhapV__4D94879B");
        });

        modelBuilder.Entity<DieuDuong>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DieuDuon__3214EC0713C0AF5E");

            entity.ToTable("DieuDuong");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.HoTen).HasMaxLength(255);

            entity.HasOne(d => d.Khoa).WithMany(p => p.DieuDuongs)
                .HasForeignKey(d => d.KhoaId)
                .HasConstraintName("FK__DieuDuong__KhoaI__49C3F6B7");
        });

        modelBuilder.Entity<GiuongBenh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GiuongBe__3214EC07273EFCE6");

            entity.ToTable("GiuongBenh");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.LoaiGiuong).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.Khoa).WithMany(p => p.GiuongBenhs)
                .HasForeignKey(d => d.KhoaId)
                .HasConstraintName("FK__GiuongBen__KhoaI__4316F928");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HoaDon__3214EC07DD321398");

            entity.ToTable("HoaDon");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BaoHiemChiTra).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BenhNhanThanhToan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Ngay).HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.BenhNhan).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.BenhNhanId)
                .HasConstraintName("FK__HoaDon__BenhNhan__5CD6CB2B");

            entity.HasOne(d => d.NhapVien).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.NhapVienId)
                .HasConstraintName("FK__HoaDon__NhapVien__5DCAEF64");
        });

        modelBuilder.Entity<KhoaPhong>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KhoaPhon__3214EC0748E6D88E");

            entity.ToTable("KhoaPhong");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.LoaiKhoa).HasMaxLength(50);
            entity.Property(e => e.TenKhoa).HasMaxLength(255);
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguoiDun__3214EC07F3177F77");

            entity.ToTable("NguoiDung");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.MatKhauHash).HasMaxLength(255);
            entity.Property(e => e.TenDangNhap).HasMaxLength(255);
            entity.Property(e => e.VaiTro).HasMaxLength(50);
        });

        modelBuilder.Entity<NhapVien>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhapVien__3214EC0711B22FA1");

            entity.ToTable("NhapVien");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.LyDoNhap).HasMaxLength(255);
            entity.Property(e => e.NgayNhap).HasColumnType("datetime");
            entity.Property(e => e.NgayXuat).HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.BenhNhan).WithMany(p => p.NhapViens)
                .HasForeignKey(d => d.BenhNhanId)
                .HasConstraintName("FK__NhapVien__BenhNh__3E52440B");

            entity.HasOne(d => d.Khoa).WithMany(p => p.NhapViens)
                .HasForeignKey(d => d.KhoaId)
                .HasConstraintName("FK__NhapVien__KhoaId__3F466844");
        });

        modelBuilder.Entity<NhatKyHeThong>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhatKyHe__3214EC07411B8B2B");

            entity.ToTable("NhatKyHeThong");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.HanhDong).HasMaxLength(255);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.ThoiGian).HasColumnType("datetime");

            entity.HasOne(d => d.NguoiDung).WithMany(p => p.NhatKyHeThongs)
                .HasForeignKey(d => d.NguoiDungId)
                .HasConstraintName("FK__NhatKyHeT__Nguoi__6477ECF3");
        });

        modelBuilder.Entity<PhauThuat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PhauThua__3214EC0732D72AE8");

            entity.ToTable("PhauThuat");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Ekip).HasMaxLength(255);
            entity.Property(e => e.LoaiPhauThuat).HasMaxLength(255);
            entity.Property(e => e.Ngay).HasColumnType("datetime");
            entity.Property(e => e.PhongMo).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.BacSiChinh).WithMany(p => p.PhauThuats)
                .HasForeignKey(d => d.BacSiChinhId)
                .HasConstraintName("FK__PhauThuat__BacSi__59063A47");

            entity.HasOne(d => d.NhapVien).WithMany(p => p.PhauThuats)
                .HasForeignKey(d => d.NhapVienId)
                .HasConstraintName("FK__PhauThuat__NhapV__5812160E");
        });

        modelBuilder.Entity<XetNghiem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__XetNghie__3214EC076C819C26");

            entity.ToTable("XetNghiem");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.KetQua).HasMaxLength(255);
            entity.Property(e => e.LoaiXetNghiem).HasMaxLength(255);
            entity.Property(e => e.Ngay).HasColumnType("datetime");

            entity.HasOne(d => d.BacSi).WithMany(p => p.XetNghiems)
                .HasForeignKey(d => d.BacSiId)
                .HasConstraintName("FK__XetNghiem__BacSi__5441852A");

            entity.HasOne(d => d.NhapVien).WithMany(p => p.XetNghiems)
                .HasForeignKey(d => d.NhapVienId)
                .HasConstraintName("FK__XetNghiem__NhapV__534D60F1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
