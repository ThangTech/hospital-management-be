namespace IdentityService.DTOs;

/// <summary>
/// DTO để đăng ký user mới
/// </summary>
public class RegisterDTO
{
    /// <summary>
    /// Tên đăng nhập (bắt buộc, unique)
    /// </summary>
    public required string TenDangNhap { get; set; }
    
    /// <summary>
    /// Mật khẩu (bắt buộc, tối thiểu 6 ký tự)
    /// </summary>
    public required string MatKhau { get; set; }
    
    /// <summary>
    /// Vai trò: Admin, BacSi, YTa, KeToan, BenhNhan
    /// </summary>
    public required string VaiTro { get; set; }
}
