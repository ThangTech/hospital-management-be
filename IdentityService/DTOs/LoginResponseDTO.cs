namespace IdentityService.DTOs;

/// <summary>
/// DTO trả về sau khi đăng nhập thành công
/// </summary>
public class LoginResponseDTO
{
    /// <summary>
    /// JWT Token để gọi API
    /// </summary>
    public required string Token { get; set; }
    
    /// <summary>
    /// Thời gian hết hạn token (UTC)
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// Thông tin user
    /// </summary>
    public required UserDTO User { get; set; }
}

/// <summary>
/// Thông tin user (không bao gồm mật khẩu)
/// </summary>
public class UserDTO
{
    public Guid Id { get; set; }
    public string? TenDangNhap { get; set; }
    public string? VaiTro { get; set; }
    
    /// <summary>
    /// Danh sách quyền của user dựa trên vai trò
    /// </summary>
    public List<string> Permissions { get; set; } = new();
}
