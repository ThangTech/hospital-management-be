namespace AdminService.DTOs;

// ===== USER MANAGEMENT DTOs =====

/// <summary>
/// DTO hiển thị User
/// </summary>
public class UserDTO
{
    public Guid Id { get; set; }
    public string? TenDangNhap { get; set; }
    public string? VaiTro { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO tạo User mới
/// </summary>
public class CreateUserDTO
{
    public required string TenDangNhap { get; set; }
    public required string MatKhau { get; set; }
    public required string VaiTro { get; set; }
}

/// <summary>
/// DTO cập nhật User
/// </summary>
public class UpdateUserDTO
{
    public Guid Id { get; set; }
    public string? TenDangNhap { get; set; }
    public string? VaiTro { get; set; }
}

/// <summary>
/// DTO reset mật khẩu
/// </summary>
public class ResetPasswordDTO
{
    public Guid UserId { get; set; }
    public required string MatKhauMoi { get; set; }
}

/// <summary>
/// DTO tìm kiếm User
/// </summary>
public class UserSearchDTO
{
    public string? TenDangNhap { get; set; }
    public string? VaiTro { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Các vai trò hợp lệ
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string BacSi = "BacSi";
    public const string YTa = "YTa";
    public const string KeToan = "KeToan";
    public const string BenhNhan = "BenhNhan";

    public static readonly string[] AllRoles = { Admin, BacSi, YTa, KeToan, BenhNhan };

    public static bool IsValid(string role) => AllRoles.Contains(role);
}
