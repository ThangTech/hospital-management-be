using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityService.Authorization;
using IdentityService.DAL;
using IdentityService.DTOs;
using IdentityService.Models;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.BLL;

/// <summary>
/// Service xử lý Authentication & Authorization
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    #region Public Methods

    /// <summary>
    /// Đăng ký user mới
    /// </summary>
    public async Task<(bool Success, string Message, UserDTO? User)> RegisterAsync(RegisterDTO dto)
    {
        // Validate vai trò
        if (!Roles.IsValidRole(dto.VaiTro))
        {
            return (false, $"Vai trò không hợp lệ. Các vai trò hợp lệ: {string.Join(", ", Roles.AllRoles)}", null);
        }

        // Kiểm tra username đã tồn tại
        if (await _userRepository.ExistsAsync(dto.TenDangNhap))
        {
            return (false, "Tên đăng nhập đã tồn tại", null);
        }

        // Validate mật khẩu
        if (dto.MatKhau.Length < 6)
        {
            return (false, "Mật khẩu phải có ít nhất 6 ký tự", null);
        }

        // Tạo user mới
        var nguoiDung = new NguoiDung
        {
            TenDangNhap = dto.TenDangNhap,
            MatKhauHash = HashPassword(dto.MatKhau),
            VaiTro = dto.VaiTro
        };

        var created = await _userRepository.CreateAsync(nguoiDung);

        var userDto = MapToUserDTO(created);
        return (true, "Đăng ký thành công", userDto);
    }

    /// <summary>
    /// Đăng nhập và trả về JWT token
    /// </summary>
    public async Task<(bool Success, string Message, LoginResponseDTO? Response)> LoginAsync(LoginDTO dto)
    {
        // Tìm user
        var user = await _userRepository.GetByTenDangNhapAsync(dto.TenDangNhap);
        if (user == null)
        {
            return (false, "Tên đăng nhập hoặc mật khẩu không đúng", null);
        }

        // Verify password
        if (!VerifyPassword(dto.MatKhau, user.MatKhauHash ?? ""))
        {
            return (false, "Tên đăng nhập hoặc mật khẩu không đúng", null);
        }

        // Generate JWT token
        var (token, expiresAt) = GenerateJwtToken(user);

        var response = new LoginResponseDTO
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = MapToUserDTO(user)
        };

        return (true, "Đăng nhập thành công", response);
    }

    /// <summary>
    /// Lấy thông tin user hiện tại
    /// </summary>
    public async Task<UserDTO?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? MapToUserDTO(user) : null;
    }

    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    public async Task<(bool Success, string Message)> ChangePasswordAsync(Guid userId, ChangePasswordDTO dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return (false, "User không tồn tại");
        }

        // Verify mật khẩu cũ
        if (!VerifyPassword(dto.MatKhauCu, user.MatKhauHash ?? ""))
        {
            return (false, "Mật khẩu cũ không đúng");
        }

        // Validate mật khẩu mới
        if (dto.MatKhauMoi.Length < 6)
        {
            return (false, "Mật khẩu mới phải có ít nhất 6 ký tự");
        }

        // Cập nhật mật khẩu
        user.MatKhauHash = HashPassword(dto.MatKhauMoi);
        await _userRepository.UpdateAsync(user);

        return (true, "Đổi mật khẩu thành công");
    }

    /// <summary>
    /// Yêu cầu reset password - tạo token
    /// Lưu ý: Trong thực tế, token này sẽ được gửi qua email/SMS
    /// </summary>
    public async Task<(bool Success, string Message, string? ResetToken)> ForgotPasswordAsync(ForgotPasswordDTO dto)
    {
        var user = await _userRepository.GetByTenDangNhapAsync(dto.TenDangNhap);
        if (user == null)
        {
            // Không tiết lộ user có tồn tại hay không (security)
            return (true, "Nếu tài khoản tồn tại, bạn sẽ nhận được hướng dẫn reset mật khẩu", null);
        }

        // Tạo reset token (trong thực tế sẽ lưu vào DB với expiry)
        var resetToken = GenerateResetToken();
        
        // TODO: Gửi token qua email/SMS
        // Tạm thời trả về token để test

        return (true, "Token reset password đã được tạo", resetToken);
    }

    /// <summary>
    /// Reset password với token
    /// Lưu ý: Đơn giản hóa - trong thực tế cần verify token từ DB
    /// </summary>
    public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDTO dto)
    {
        var user = await _userRepository.GetByTenDangNhapAsync(dto.TenDangNhap);
        if (user == null)
        {
            return (false, "Token không hợp lệ hoặc đã hết hạn");
        }

        // TODO: Verify reset token từ DB
        // Tạm thời chỉ validate token không rỗng
        if (string.IsNullOrEmpty(dto.ResetToken))
        {
            return (false, "Token không hợp lệ");
        }

        // Validate mật khẩu mới
        if (dto.MatKhauMoi.Length < 6)
        {
            return (false, "Mật khẩu mới phải có ít nhất 6 ký tự");
        }

        // Cập nhật mật khẩu
        user.MatKhauHash = HashPassword(dto.MatKhauMoi);
        await _userRepository.UpdateAsync(user);

        return (true, "Đặt lại mật khẩu thành công");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Hash password using SHA256 (trong thực tế nên dùng BCrypt)
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Verify password
    /// </summary>
    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }

    /// <summary>
    /// Generate JWT Token
    /// </summary>
    private (string Token, DateTime ExpiresAt) GenerateJwtToken(NguoiDung user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHere_MustBe32Chars!";
        var issuer = jwtSettings["Issuer"] ?? "IdentityService";
        var audience = jwtSettings["Audience"] ?? "HospitalManagement";
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        // Lấy permissions từ vai trò
        var permissions = RolePermissions.GetPermissions(user.VaiTro ?? "");

        // Tạo claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.TenDangNhap ?? ""),
            new(ClaimTypes.Role, user.VaiTro ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Thêm permissions vào claims
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permissions", permission));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    /// <summary>
    /// Tạo reset token ngẫu nhiên
    /// </summary>
    private static string GenerateResetToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Map NguoiDung entity sang UserDTO
    /// </summary>
    private static UserDTO MapToUserDTO(NguoiDung user)
    {
        return new UserDTO
        {
            Id = user.Id,
            TenDangNhap = user.TenDangNhap,
            VaiTro = user.VaiTro,
            Permissions = RolePermissions.GetPermissions(user.VaiTro ?? "")
        };
    }

    #endregion
}
