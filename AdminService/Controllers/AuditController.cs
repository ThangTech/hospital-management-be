using AdminService.BLL;
using AdminService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.Controllers;

/// <summary>
/// Controller xem Audit Logs
/// Chỉ Admin mới có quyền truy cập
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    /// <summary>
    /// Lấy danh sách Nhật ký hệ thống
    /// POST /api/audit/system-logs
    /// </summary>
    [HttpPost("system-logs")]
    public IActionResult GetSystemLogs([FromBody] AuditSearchDTO search)
    {
        try
        {
            var result = _auditService.GetNhatKyHeThong(search);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy danh sách Audit Hồ sơ bệnh án
    /// POST /api/audit/medical-record-logs
    /// </summary>
    [HttpPost("medical-record-logs")]
    public IActionResult GetMedicalRecordLogs([FromBody] AuditSearchDTO search)
    {
        try
        {
            var result = _auditService.GetAuditHoSoBenhAn(search);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy nhật ký hệ thống theo User
    /// GET /api/audit/by-user/{userId}
    /// </summary>
    [HttpGet("by-user/{userId:guid}")]
    public IActionResult GetByUser(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var search = new AuditSearchDTO
            {
                NguoiDungId = userId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = _auditService.GetNhatKyHeThong(search);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy nhật ký theo khoảng thời gian
    /// GET /api/audit/by-date-range?tuNgay=...&denNgay=...
    /// </summary>
    [HttpGet("by-date-range")]
    public IActionResult GetByDateRange(
        [FromQuery] DateTime tuNgay, 
        [FromQuery] DateTime denNgay,
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var search = new AuditSearchDTO
            {
                TuNgay = tuNgay,
                DenNgay = denNgay,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = _auditService.GetNhatKyHeThong(search);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}
