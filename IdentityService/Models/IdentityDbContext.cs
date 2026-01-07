using Microsoft.EntityFrameworkCore;

namespace IdentityService.Models;

/// <summary>
/// DbContext cho Identity Service
/// Chỉ quản lý bảng NguoiDung
/// </summary>
public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) 
        : base(options)
    {
    }

    public DbSet<NguoiDung> NguoiDungs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.ToTable("NguoiDung");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())");
            
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(255);
            
            entity.Property(e => e.MatKhauHash)
                .HasMaxLength(255);
            
            entity.Property(e => e.VaiTro)
                .HasMaxLength(50);
        });
    }
}
