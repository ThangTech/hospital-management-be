namespace IdentityService.Models;

/// <summary>
/// Entity PasswordResetToken - quản lý OTP và token reset mật khẩu
/// </summary>
public class PasswordResetToken
{
    public Guid Id { get; set; }

    /// <summary>
    /// User cần reset mật khẩu
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Mã OTP 6 số - gửi tới email người dùng
    /// </summary>
    public string? OtpCode { get; set; }

    /// <summary>
    /// Token reset 1 lần dùng - cấp sau khi OTP xác thực thành công
    /// </summary>
    public string? ResetToken { get; set; }

    /// <summary>
    /// Thời hạn OTP (mặc định: 5 phút)
    /// </summary>
    public DateTime? OtpExpiry { get; set; }

    /// <summary>
    /// Thời hạn ResetToken (mặc định: 10 phút)
    /// </summary>
    public DateTime? TokenExpiry { get; set; }

    /// <summary>
    /// Số lần nhập OTP sai (tối đa 5 lần)
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Đã sử dụng / đã bị vô hiệu
    /// </summary>
    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }
}
