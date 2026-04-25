using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
    private readonly IPasswordResetRepository _passwordResetRepository;
    private readonly IConfiguration _configuration;
    private const int MaxOtpAttempts = 5;

    public AuthService(
        IUserRepository userRepository,
        IPasswordResetRepository passwordResetRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordResetRepository = passwordResetRepository;
        _configuration = configuration;
    }

    #region Register & Login

    /// <summary>
    /// Đăng ký user mới (Email bắt buộc)
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

        // Validate định dạng email
        if (!IsValidEmail(dto.Email))
        {
            return (false, "Định dạng email không hợp lệ", null);
        }

        // Kiểm tra email đã tồn tại
        if (await _userRepository.EmailExistsAsync(dto.Email))
        {
            return (false, "Email đã được sử dụng bởi tài khoản khác", null);
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
            VaiTro      = dto.VaiTro,
            Email       = dto.Email.ToLowerInvariant().Trim()
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
            Token     = token,
            ExpiresAt = expiresAt,
            User      = MapToUserDTO(user)
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
    /// Đổi mật khẩu (yêu cầu đăng nhập, cần nhập mật khẩu cũ)
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

    #endregion

    #region Forgot Password Flow (3 bước)

    /// <summary>
    /// Bước 1: Tạo OTP 6 số và "gửi" tới email người dùng.
    /// Luôn trả về thông báo chung để tránh tiết lộ email có tồn tại không.
    /// Dev mode: trả OtpForDev trong response.
    /// </summary>
    public async Task<(bool Success, string Message, string? OtpForDev)> ForgotPasswordAsync(ForgotPasswordDTO dto)
    {
        var email = dto.Email.ToLowerInvariant().Trim();
        var user  = await _userRepository.GetByEmailAsync(email);

        // Luôn trả về response thành công - không tiết lộ email tồn tại hay không
        if (user == null)
        {
            return (true,
                "Nếu email tồn tại trong hệ thống, bạn sẽ nhận được mã OTP trong vài phút.",
                null);
        }

        // Tạo OTP và lưu vào DB (vô hiệu OTP cũ tự động)
        var otpCode = await _passwordResetRepository.CreateOtpAsync(user.Id);

        // TODO: Tích hợp email service thật (SendGrid, SMTP...)
        // Hiện tại: giả lập gửi email, trả OTP trong response cho môi trường dev
        var isDevelopment = _configuration["ASPNETCORE_ENVIRONMENT"] == "Development"
                         || _configuration["ASPNETCORE_ENVIRONMENT"] == null;

        return (true,
            "Nếu email tồn tại trong hệ thống, bạn sẽ nhận được mã OTP trong vài phút.",
            isDevelopment ? otpCode : null); // Production: KHÔNG trả OTP
    }

    /// <summary>
    /// Bước 2: Xác minh OTP.
    /// - Tối đa 5 lần thử sai → OTP bị vô hiệu
    /// - OTP hết hạn sau 5 phút
    /// - Nếu đúng → tạo ResetToken 1 lần dùng (hết hạn 10 phút)
    /// </summary>
    public async Task<(bool Success, string Message, string? ResetToken)> VerifyResetOtpAsync(VerifyResetOtpDTO dto)
    {
        var email = dto.Email.ToLowerInvariant().Trim();
        var user  = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            return (false, "OTP không hợp lệ hoặc đã hết hạn", null);
        }

        // Lấy OTP active từ DB
        var otpRecord = await _passwordResetRepository.GetActiveOtpAsync(user.Id);

        if (otpRecord == null)
        {
            return (false, "OTP không hợp lệ hoặc đã hết hạn. Vui lòng yêu cầu gửi OTP mới.", null);
        }

        // Kiểm tra số lần thử
        if (otpRecord.AttemptCount >= MaxOtpAttempts)
        {
            await _passwordResetRepository.InvalidateTokenAsync(otpRecord.Id);
            return (false, "Đã vượt quá số lần thử. Vui lòng yêu cầu gửi OTP mới.", null);
        }

        // Kiểm tra OTP có đúng không
        if (otpRecord.OtpCode != dto.OtpCode.Trim())
        {
            var attempts = await _passwordResetRepository.IncrementAttemptAsync(otpRecord.Id);
            var remaining = MaxOtpAttempts - attempts;

            if (remaining <= 0)
            {
                await _passwordResetRepository.InvalidateTokenAsync(otpRecord.Id);
                return (false, "OTP không đúng. Đã vượt quá số lần thử. Vui lòng yêu cầu gửi OTP mới.", null);
            }

            return (false, $"OTP không đúng. Còn {remaining} lần thử.", null);
        }

        // OTP hợp lệ - vô hiệu OTP đã dùng
        await _passwordResetRepository.InvalidateTokenAsync(otpRecord.Id);

        // Tạo ResetToken 1 lần dùng (hết hạn 10 phút)
        var resetToken = await _passwordResetRepository.CreateResetTokenAsync(user.Id);

        return (true, "Xác minh OTP thành công. Vui lòng đặt lại mật khẩu trong 10 phút.", resetToken);
    }

    /// <summary>
    /// Bước 3: Đặt lại mật khẩu bằng ResetToken.
    /// Token chỉ dùng 1 lần, hết hạn sau 10 phút kể từ khi verify OTP.
    /// </summary>
    public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ResetToken))
        {
            return (false, "Token không hợp lệ");
        }

        // Verify token từ DB (chưa dùng + chưa hết hạn)
        var tokenRecord = await _passwordResetRepository.GetActiveResetTokenAsync(dto.ResetToken);

        if (tokenRecord == null)
        {
            return (false, "Token không hợp lệ hoặc đã hết hạn. Vui lòng thực hiện lại quy trình quên mật khẩu.");
        }

        // Validate mật khẩu mới
        if (dto.MatKhauMoi.Length < 6)
        {
            return (false, "Mật khẩu mới phải có ít nhất 6 ký tự");
        }

        // Lấy user
        var user = await _userRepository.GetByIdAsync(tokenRecord.UserId);
        if (user == null)
        {
            return (false, "Tài khoản không tồn tại");
        }

        // Cập nhật mật khẩu
        user.MatKhauHash = HashPassword(dto.MatKhauMoi);
        await _userRepository.UpdateAsync(user);

        // Đánh dấu token đã dùng (1 lần dùng duy nhất)
        await _passwordResetRepository.MarkTokenUsedAsync(tokenRecord.Id);

        // Vô hiệu tất cả token còn lại của user (nếu có)
        await _passwordResetRepository.InvalidateAllUserTokensAsync(user.Id);

        return (true, "Đặt lại mật khẩu thành công. Vui lòng đăng nhập bằng mật khẩu mới.");
    }

    #endregion

    #region Admin

    /// <summary>
    /// Admin reset mật khẩu cho bất kỳ user nào (không cần OTP).
    /// Chỉ dành cho role Admin.
    /// </summary>
    public async Task<(bool Success, string Message)> AdminResetPasswordAsync(
        Guid adminId, AdminResetPasswordDTO dto)
    {
        // Kiểm tra target user tồn tại
        var targetUser = await _userRepository.GetByIdAsync(dto.TargetUserId);
        if (targetUser == null)
        {
            return (false, "Người dùng không tồn tại");
        }

        // Không cho phép Admin tự reset chính mình qua endpoint này
        // (Dùng /change-password thay thế)
        if (targetUser.Id == adminId)
        {
            return (false, "Vui lòng dùng chức năng Đổi mật khẩu để thay đổi mật khẩu của chính bạn");
        }

        // Validate mật khẩu mới
        if (dto.MatKhauMoi.Length < 6)
        {
            return (false, "Mật khẩu mới phải có ít nhất 6 ký tự");
        }

        var newHash = HashPassword(dto.MatKhauMoi);
        var success = await _userRepository.AdminResetPasswordAsync(dto.TargetUserId, newHash);

        if (!success)
        {
            return (false, "Không thể reset mật khẩu. Vui lòng thử lại.");
        }

        // Vô hiệu tất cả token reset đang active của user đó
        await _passwordResetRepository.InvalidateAllUserTokensAsync(dto.TargetUserId);

        return (true, $"Đã reset mật khẩu thành công cho tài khoản '{targetUser.TenDangNhap}'");
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Hash password dùng SHA256 (đơn giản cho demo)
    /// Production: nên dùng BCrypt hoặc Argon2
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash  = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }

    /// <summary>
    /// Validate định dạng email cơ bản
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        return Regex.IsMatch(email.Trim(),
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Generate JWT Token với claims + permissions
    /// </summary>
    private (string Token, DateTime ExpiresAt) GenerateJwtToken(NguoiDung user)
    {
        var jwtSettings    = _configuration.GetSection("JwtSettings");
        var secretKey      = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHere_MustBe32Chars!";
        var issuer         = jwtSettings["Issuer"] ?? "IdentityService";
        var audience       = jwtSettings["Audience"] ?? "HospitalManagement";
        var expiryMinutes  = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt   = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var permissions = RolePermissions.GetPermissions(user.VaiTro ?? "");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,  user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.TenDangNhap ?? ""),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(ClaimTypes.Role,              user.VaiTro ?? ""),
            new(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString())
        };

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permissions", permission));
        }

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            expiresAt,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    private static UserDTO MapToUserDTO(NguoiDung user)
    {
        return new UserDTO
        {
            Id          = user.Id,
            TenDangNhap = user.TenDangNhap,
            VaiTro      = user.VaiTro,
            Email       = user.Email,
            Permissions = RolePermissions.GetPermissions(user.VaiTro ?? "")
        };
    }

    #endregion
}
