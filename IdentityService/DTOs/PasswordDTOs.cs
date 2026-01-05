namespace IdentityService.DTOs;

/// <summary>
/// DTO để yêu cầu đặt lại mật khẩu
/// </summary>
public class ForgotPasswordDTO
{
    /// <summary>
    /// Tên đăng nhập cần reset mật khẩu
    /// </summary>
    public required string TenDangNhap { get; set; }
}

/// <summary>
/// DTO để đặt lại mật khẩu mới
/// </summary>
public class ResetPasswordDTO
{
    /// <summary>
    /// Tên đăng nhập
    /// </summary>
    public required string TenDangNhap { get; set; }
    
    /// <summary>
    /// Token reset password (được gửi qua email/SMS)
    /// </summary>
    public required string ResetToken { get; set; }
    
    /// <summary>
    /// Mật khẩu mới
    /// </summary>
    public required string MatKhauMoi { get; set; }
}

/// <summary>
/// DTO để đổi mật khẩu (khi đã đăng nhập)
/// </summary>
public class ChangePasswordDTO
{
    /// <summary>
    /// Mật khẩu hiện tại
    /// </summary>
    public required string MatKhauCu { get; set; }
    
    /// <summary>
    /// Mật khẩu mới
    /// </summary>
    public required string MatKhauMoi { get; set; }
}
