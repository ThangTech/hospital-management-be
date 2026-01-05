namespace AdminService.DTOs;

// ===== AUDIT DTOs =====

/// <summary>
/// DTO hiển thị Nhật ký hệ thống
/// </summary>
public class NhatKyHeThongDTO
{
    public Guid Id { get; set; }
    public Guid? NguoiDungId { get; set; }
    public string? TenNguoiDung { get; set; }
    public string? HanhDong { get; set; }
    public DateTime? ThoiGian { get; set; }
    public string? MoTa { get; set; }
}

/// <summary>
/// DTO hiển thị Audit Hồ sơ bệnh án
/// </summary>
public class AuditHoSoBenhAnDTO
{
    public Guid Id { get; set; }
    public Guid? HoSoBenhAnId { get; set; }
    public string? HanhDong { get; set; }
    public string? ChanDoanCu { get; set; }
    public string? KetQuaCu { get; set; }
    public Guid? NguoiDungId { get; set; }
    public string? TenNguoiSua { get; set; }
    public DateTime? ThoiGianSua { get; set; }
}

/// <summary>
/// DTO tìm kiếm Audit
/// </summary>
public class AuditSearchDTO
{
    public Guid? NguoiDungId { get; set; }
    public string? HanhDong { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Kết quả phân trang
/// </summary>
public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
}
