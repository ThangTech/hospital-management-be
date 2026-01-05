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

    /// <summary>
    /// Yêu cầu reset password
    /// POST /api/auth/forgot-password
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
    {
        var (success, message, resetToken) = await _authService.ForgotPasswordAsync(dto);
        
        // Luôn trả về success để không tiết lộ user có tồn tại hay không
        return Ok(new { 
            success = true, 
            message,
            // Chỉ trả token để test, trong production không nên trả
            resetToken = resetToken 
        });
    }

    /// <summary>
    /// Reset password với token
    /// POST /api/auth/reset-password
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
    [Authorize] // <-- Yêu cầu đăng nhập
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
    /// Đổi mật khẩu
    /// POST /api/auth/change-password
    /// Yêu cầu: Đã đăng nhập
    /// </summary>
    [HttpPost("change-password")]
    [Authorize] // <-- Yêu cầu đăng nhập
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
    /// Ví dụ endpoint chỉ Admin mới được truy cập
    /// GET /api/auth/admin-only
    /// Yêu cầu: Role = Admin
    /// </summary>
    [HttpGet("admin-only")]
    [Authorize(Roles = Roles.Admin)] // <-- Chỉ Admin
    public IActionResult AdminOnly()
    {
        return Ok(new { 
            success = true, 
            message = "Bạn là Admin! Có thể truy cập endpoint này." 
        });
    }

    #endregion

    #region Permission-Based Endpoints (Theo quyền cụ thể)

    /// <summary>
    /// Ví dụ endpoint cần quyền BenhNhan.Xem
    /// GET /api/auth/demo-permission
    /// Yêu cầu: Permission = BenhNhan.Xem
    /// </summary>
    [HttpGet("demo-permission")]
    [HasPermission(Permissions.BenhNhan_Xem)] // <-- Cần quyền cụ thể
    public IActionResult DemoPermission()
    {
        return Ok(new { 
            success = true, 
            message = "Bạn có quyền xem bệnh nhân!" 
        });
    }

    /// <summary>
    /// Ví dụ endpoint cho nhiều roles
    /// GET /api/auth/medical-staff
    /// Yêu cầu: Role = BacSi HOẶC YTa
    /// </summary>
    [HttpGet("medical-staff")]
    [Authorize(Roles = $"{Roles.BacSi},{Roles.YTa},{Roles.Admin}")] // <-- Nhiều roles
    public IActionResult MedicalStaffOnly()
    {
        return Ok(new { 
            success = true, 
            message = "Bạn là nhân viên y tế (Bác sĩ hoặc Y tá)!" 
        });
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Lấy UserId từ JWT token
    /// </summary>
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
