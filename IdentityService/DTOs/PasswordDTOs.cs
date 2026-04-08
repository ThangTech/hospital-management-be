using System.ComponentModel.DataAnnotations;

namespace IdentityService.DTOs;

/// <summary>
/// DTO để yêu cầu gửi OTP reset mật khẩu
/// Bước 1: User nhập email → hệ thống gửi OTP
/// </summary>
public class ForgotPasswordDTO
{
    /// <summary>
    /// Email đã đăng ký của tài khoản
    /// </summary>
    [EmailAddress]
    public required string Email { get; set; }
}

/// <summary>
/// DTO để xác minh OTP reset mật khẩu
/// Bước 2: User nhập OTP nhận được qua email
/// </summary>
public class VerifyResetOtpDTO
{
    /// <summary>
    /// Email đã dùng ở bước 1
    /// </summary>
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// Mã OTP 6 số nhận qua email
    /// </summary>
    public required string OtpCode { get; set; }
}

/// <summary>
/// DTO để đặt lại mật khẩu mới
/// Bước 3: Dùng ResetToken nhận được sau khi verify OTP
/// </summary>
public class ResetPasswordDTO
{
    /// <summary>
    /// Token 1 lần dùng (nhận được sau khi verify OTP thành công)
    /// </summary>
    public required string ResetToken { get; set; }

    /// <summary>
    /// Mật khẩu mới (tối thiểu 6 ký tự)
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
    /// Mật khẩu mới (tối thiểu 6 ký tự)
    /// </summary>
    public required string MatKhauMoi { get; set; }
}

/// <summary>
/// DTO để Admin reset mật khẩu cho user khác (không cần OTP)
/// </summary>
public class AdminResetPasswordDTO
{
    /// <summary>
    /// ID của user cần reset mật khẩu
    /// </summary>
    public required Guid TargetUserId { get; set; }

    /// <summary>
    /// Mật khẩu mới (tối thiểu 6 ký tự)
    /// </summary>
    public required string MatKhauMoi { get; set; }
}
