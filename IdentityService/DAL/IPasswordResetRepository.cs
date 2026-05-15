using IdentityService.Models;

namespace IdentityService.DAL;

/// <summary>
/// Interface cho Password Reset Repository
/// Quản lý OTP và ResetToken cho luồng quên mật khẩu
/// </summary>
public interface IPasswordResetRepository
{
    /// <summary>
    /// Tạo OTP mới cho user (6 số, hết hạn sau 5 phút)
    /// Tự động vô hiệu các OTP cũ chưa dùng
    /// </summary>
    Task<string> CreateOtpAsync(Guid userId);

    /// <summary>
    /// Lấy OTP đang active của user (chưa dùng, chưa hết hạn)
    /// </summary>
    Task<PasswordResetToken?> GetActiveOtpAsync(Guid userId);

    /// <summary>
    /// Tăng số lần thử OTP sai
    /// </summary>
    /// <returns>Số lần thử sau khi tăng</returns>
    Task<int> IncrementAttemptAsync(Guid tokenId);

    /// <summary>
    /// Vô hiệu một token cụ thể
    /// </summary>
    Task InvalidateTokenAsync(Guid tokenId);

    /// <summary>
    /// Vô hiệu tất cả token/OTP đang active của user
    /// </summary>
    Task InvalidateAllUserTokensAsync(Guid userId);

    /// <summary>
    /// Tạo ResetToken sau khi OTP xác thực thành công (hết hạn sau 10 phút)
    /// </summary>
    /// <returns>Chuỗi ResetToken ngẫu nhiên</returns>
    Task<string> CreateResetTokenAsync(Guid userId);

    /// <summary>
    /// Lấy ResetToken chưa dùng, chưa hết hạn theo chuỗi token
    /// </summary>
    Task<PasswordResetToken?> GetActiveResetTokenAsync(string resetToken);

    /// <summary>
    /// Đánh dấu ResetToken đã được sử dụng (1 lần dùng duy nhất)
    /// </summary>
    Task MarkTokenUsedAsync(Guid tokenId);
}
