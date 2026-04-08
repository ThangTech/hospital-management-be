using IdentityService.Models;

namespace IdentityService.DAL;

/// <summary>
/// Interface cho User Repository
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Lấy user theo ID
    /// </summary>
    Task<NguoiDung?> GetByIdAsync(Guid id);

    /// <summary>
    /// Lấy user theo tên đăng nhập
    /// </summary>
    Task<NguoiDung?> GetByTenDangNhapAsync(string tenDangNhap);

    /// <summary>
    /// Lấy user theo email
    /// </summary>
    Task<NguoiDung?> GetByEmailAsync(string email);

    /// <summary>
    /// Kiểm tra tên đăng nhập đã tồn tại chưa
    /// </summary>
    Task<bool> ExistsAsync(string tenDangNhap);

    /// <summary>
    /// Kiểm tra email đã tồn tại chưa
    /// </summary>
    Task<bool> EmailExistsAsync(string email);

    /// <summary>
    /// Thêm user mới
    /// </summary>
    Task<NguoiDung> CreateAsync(NguoiDung nguoiDung);

    /// <summary>
    /// Cập nhật user
    /// </summary>
    Task<NguoiDung> UpdateAsync(NguoiDung nguoiDung);

    /// <summary>
    /// Admin reset mật khẩu cho user khác (không cần OTP)
    /// </summary>
    Task<bool> AdminResetPasswordAsync(Guid userId, string newPasswordHash);

    /// <summary>
    /// Lấy tất cả users (Admin only)
    /// </summary>
    Task<IEnumerable<NguoiDung>> GetAllAsync();
}
