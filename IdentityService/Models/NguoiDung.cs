namespace IdentityService.Models;

/// <summary>
/// Entity NguoiDung - scaffold từ database hospital_manage
/// </summary>
public partial class NguoiDung
{
    public Guid Id { get; set; }

    public string? TenDangNhap { get; set; }

    public string? MatKhauHash { get; set; }

    public string? VaiTro { get; set; }

    /// <summary>
    /// Email dùng để nhận OTP khi quên mật khẩu (bắt buộc)
    /// </summary>
    public string? Email { get; set; }
}
