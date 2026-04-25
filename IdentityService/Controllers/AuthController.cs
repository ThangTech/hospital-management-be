using System.Security.Claims;
using IdentityService.Authorization;
using IdentityService.BLL;
using IdentityService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

/// <summary>
/// Controller xử lý Authentication & Authorization
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    #region Public Endpoints (Không cần đăng nhập)

    /// <summary>
    /// Đăng ký user mới
    /// POST /api/auth/register
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        var (success, message, user) = await _authService.RegisterAsync(dto);

        if (!success)
        {
            return BadRequest(new { success = false, message });
        }

        return Ok(new { success = true, message, data = user });
    }

    /// <summary>
    /// Đăng nhập
    /// POST /api/auth/login
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var (success, message, response) = await _authService.LoginAsync(dto);

        if (!success)
        {
            return Unauthorized(new { success = false, message });
        }

        return Ok(new { success = true, message, data = response });
    }

    #endregion

    #region Forgot Password Flow (3 bước - Không cần đăng nhập)

    /// <summary>
    /// Bước 1: Yêu cầu gửi OTP reset mật khẩu tới email
    /// POST /api/auth/forgot-password
    /// Body: { "email": "user@example.com" }
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
    {
        var (success, message, otpForDev) = await _authService.ForgotPasswordAsync(dto);

        // Luôn trả 200 để không tiết lộ email có tồn tại không
        return Ok(new
        {
            success = true,
            message,
            // otpForDev chỉ có giá trị khi chạy môi trường Development
            // Production: field này sẽ là null
            otpForDev
        });
    }

    /// <summary>
    /// Bước 2: Xác minh OTP (tối đa 5 lần thử, hết hạn sau 5 phút)
    /// POST /api/auth/verify-reset-otp
    /// Body: { "email": "user@example.com", "otpCode": "123456" }
    /// Response: { resetToken } nếu thành công
    /// </summary>
    [HttpPost("verify-reset-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyResetOtp([FromBody] VerifyResetOtpDTO dto)
    {
        var (success, message, resetToken) = await _authService.VerifyResetOtpAsync(dto);

        if (!success)
        {
            return BadRequest(new { success = false, message });
        }

        return Ok(new
        {
            success = true,
            message,
            data = new { resetToken }
        });
    }

    /// <summary>
    /// Bước 3: Đặt lại mật khẩu bằng ResetToken (1 lần dùng, hết hạn 10 phút)
    /// POST /api/auth/reset-password
    /// Body: { "resetToken": "...", "matKhauMoi": "newPassword" }
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
    {
        var (success, message) = await _authService.ResetPasswordAsync(dto);

        if (!success)
        {
            return BadRequest(new { success = false, message });
        }

        return Ok(new { success = true, message });
    }

    #endregion

    #region Protected Endpoints (Cần đăng nhập)

    /// <summary>
    /// Lấy thông tin user đang đăng nhập
    /// GET /api/auth/me
    /// Yêu cầu: Đã đăng nhập (có JWT token)
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { success = false, message = "Token không hợp lệ" });
        }

        var user = await _authService.GetCurrentUserAsync(userId.Value);
        if (user == null)
        {
            return NotFound(new { success = false, message = "User không tồn tại" });
        }

        return Ok(new { success = true, data = user });
    }

    /// <summary>
    /// Đổi mật khẩu (khi đã đăng nhập, cần nhập mật khẩu cũ)
    /// POST /api/auth/change-password
    /// Yêu cầu: Đã đăng nhập
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { success = false, message = "Token không hợp lệ" });
        }

        var (success, message) = await _authService.ChangePasswordAsync(userId.Value, dto);

        if (!success)
        {
            return BadRequest(new { success = false, message });
        }

        return Ok(new { success = true, message });
    }

    #endregion

    #region Admin Only Endpoints (Chỉ Admin)

    /// <summary>
    /// Admin reset mật khẩu cho bất kỳ user nào (không cần OTP)
    /// POST /api/auth/admin-reset-password
    /// Yêu cầu: Role = Admin
    /// Body: { "targetUserId": "guid", "matKhauMoi": "newPassword" }
    /// </summary>
    [HttpPost("admin-reset-password")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> AdminResetPassword([FromBody] AdminResetPasswordDTO dto)
    {
        var adminId = GetCurrentUserId();
        if (adminId == null)
        {
            return Unauthorized(new { success = false, message = "Token không hợp lệ" });
        }

        var (success, message) = await _authService.AdminResetPasswordAsync(adminId.Value, dto);

        if (!success)
        {
            return BadRequest(new { success = false, message });
        }

        return Ok(new { success = true, message });
    }

    /// <summary>
    /// Ví dụ endpoint chỉ Admin mới được truy cập
    /// GET /api/auth/admin-only
    /// </summary>
    [HttpGet("admin-only")]
    [Authorize(Roles = Roles.Admin)]
    public IActionResult AdminOnly()
    {
        return Ok(new
        {
            success = true,
            message = "Bạn là Admin! Có thể truy cập endpoint này."
        });
    }

    #endregion

    #region Permission-Based Endpoints

    /// <summary>
    /// Ví dụ endpoint cần quyền BenhNhan.Xem
    /// GET /api/auth/demo-permission
    /// </summary>
    [HttpGet("demo-permission")]
    [HasPermission(Permissions.BenhNhan_Xem)]
    public IActionResult DemoPermission()
    {
        return Ok(new
        {
            success = true,
            message = "Bạn có quyền xem bệnh nhân!"
        });
    }

    /// <summary>
    /// Ví dụ endpoint cho nhân viên y tế
    /// GET /api/auth/medical-staff
    /// </summary>
    [HttpGet("medical-staff")]
    [Authorize(Roles = $"{Roles.BacSi},{Roles.YTa},{Roles.Admin}")]
    public IActionResult MedicalStaffOnly()
    {
        return Ok(new
        {
            success = true,
            message = "Bạn là nhân viên y tế (Bác sĩ hoặc Y tá)!"
        });
    }

    #endregion

    #region Helper Methods

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                       ?? User.FindFirst("sub");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }

        return userId;
    }

    #endregion
}
