using IdentityService.DTOs;

namespace IdentityService.BLL;

/// <summary>
/// Interface cho Authentication Service
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Đăng ký user mới (yêu cầu Email bắt buộc)
    /// </summary>
    Task<(bool Success, string Message, UserDTO? User)> RegisterAsync(RegisterDTO dto);

    /// <summary>
    /// Đăng nhập và trả về JWT token
    /// </summary>
    Task<(bool Success, string Message, LoginResponseDTO? Response)> LoginAsync(LoginDTO dto);

    /// <summary>
    /// Lấy thông tin user hiện tại
    /// </summary>
    Task<UserDTO?> GetCurrentUserAsync(Guid userId);

    /// <summary>
    /// Đổi mật khẩu (yêu cầu đăng nhập)
    /// </summary>
    Task<(bool Success, string Message)> ChangePasswordAsync(Guid userId, ChangePasswordDTO dto);

    // ===== FORGOT PASSWORD FLOW (3 bước) =====

    /// <summary>
    /// Bước 1: Yêu cầu reset mật khẩu - tạo OTP 6 số gửi tới email
    /// </summary>
    Task<(bool Success, string Message, string? OtpForDev)> ForgotPasswordAsync(ForgotPasswordDTO dto);

    /// <summary>
    /// Bước 2: Xác minh OTP - trả về ResetToken nếu hợp lệ
    /// </summary>
    Task<(bool Success, string Message, string? ResetToken)> VerifyResetOtpAsync(VerifyResetOtpDTO dto);

    /// <summary>
    /// Bước 3: Đặt lại mật khẩu bằng ResetToken (1 lần dùng, hết hạn 10 phút)
    /// </summary>
    Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDTO dto);

    // ===== ADMIN =====

    /// <summary>
    /// Admin reset mật khẩu cho bất kỳ user nào (không cần OTP)
    /// </summary>
    Task<(bool Success, string Message)> AdminResetPasswordAsync(Guid adminId, AdminResetPasswordDTO dto);
}
