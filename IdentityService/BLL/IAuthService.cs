using IdentityService.DTOs;

namespace IdentityService.BLL;

/// <summary>
/// Interface cho Authentication Service
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Đăng ký user mới
    /// </summary>
    Task<(bool Success, string Message, UserDTO? User)> RegisterAsync(RegisterDTO dto);
    
    /// <summary>
    /// Đăng nhập
    /// </summary>
    Task<(bool Success, string Message, LoginResponseDTO? Response)> LoginAsync(LoginDTO dto);
    
    /// <summary>
    /// Lấy thông tin user hiện tại
    /// </summary>
    Task<UserDTO?> GetCurrentUserAsync(Guid userId);
    
    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    Task<(bool Success, string Message)> ChangePasswordAsync(Guid userId, ChangePasswordDTO dto);
    
    /// <summary>
    /// Yêu cầu reset password (tạo token)
    /// </summary>
    Task<(bool Success, string Message, string? ResetToken)> ForgotPasswordAsync(ForgotPasswordDTO dto);
    
    /// <summary>
    /// Reset password với token
    /// </summary>
    Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDTO dto);
}
