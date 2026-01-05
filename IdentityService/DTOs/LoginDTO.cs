namespace IdentityService.DTOs;

/// <summary>
/// DTO để đăng nhập
/// </summary>
public class LoginDTO
{
    /// <summary>
    /// Tên đăng nhập
    /// </summary>
    public required string TenDangNhap { get; set; }
    
    /// <summary>
    /// Mật khẩu
    /// </summary>
    public required string MatKhau { get; set; }
}
