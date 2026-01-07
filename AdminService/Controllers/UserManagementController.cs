using AdminService.BLL;
using AdminService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.Controllers;

/// <summary>
/// Controller quản lý Users
/// Chỉ Admin mới có quyền truy cập
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserManagementController : ControllerBase
{
    private readonly IUserManagementService _userService;

    public UserManagementController(IUserManagementService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Lấy tất cả users
    /// GET /api/usermanagement
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var users = _userService.GetAll();
            return Ok(new { success = true, data = users });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Tìm kiếm users
    /// POST /api/usermanagement/search
    /// </summary>
    [HttpPost("search")]
    public IActionResult Search([FromBody] UserSearchDTO search)
    {
        try
        {
            var result = _userService.Search(search);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy user theo ID
    /// GET /api/usermanagement/{id}
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        try
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound(new { success = false, message = "User không tồn tại" });

            return Ok(new { success = true, data = user });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Tạo user mới
    /// POST /api/usermanagement
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateUserDTO dto)
    {
        try
        {
            var (success, message, user) = _userService.Create(dto);
            
            if (!success)
                return BadRequest(new { success = false, message });

            return StatusCode(201, new { success = true, message, data = user });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật user
    /// PUT /api/usermanagement/{id}
    /// </summary>
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateUserDTO dto)
    {
        try
        {
            dto.Id = id;
            var (success, message, user) = _userService.Update(dto);
            
            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message, data = user });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa user
    /// DELETE /api/usermanagement/{id}
    /// </summary>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var (success, message) = _userService.Delete(id);
            
            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Reset mật khẩu cho user
    /// POST /api/usermanagement/reset-password
    /// </summary>
    [HttpPost("reset-password")]
    public IActionResult ResetPassword([FromBody] ResetPasswordDTO dto)
    {
        try
        {
            var (success, message) = _userService.ResetPassword(dto);
            
            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy danh sách vai trò hợp lệ
    /// GET /api/usermanagement/roles
    /// </summary>
    [HttpGet("roles")]
    public IActionResult GetRoles()
    {
        return Ok(new { success = true, data = Roles.AllRoles });
    }
}
