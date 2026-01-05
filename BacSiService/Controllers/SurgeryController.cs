using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace BacSiService.Controllers
{
    /// <summary>
    /// Controller quản lý Phẫu thuật
    /// Quyền: Admin, BacSi
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SurgeryController : ControllerBase
    {
        private readonly ISurgeryService _service;

        public SurgeryController(ISurgeryService service)
        {
            _service = service;
        }

        // Helper: Lấy user info từ JWT claims
        private (Guid? userId, string? userName) GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value 
                        ?? User.FindFirst("unique_name")?.Value;
            
            Guid? userId = null;
            if (Guid.TryParse(userIdClaim, out var parsedId))
                userId = parsedId;
            
            return (userId, userName);
        }

        /// <summary>
        /// Lấy tất cả lịch phẫu thuật
        /// </summary>
        [HttpGet("get-all-surgery")]
        [Authorize(Roles = "Admin,BacSi")]
        public IActionResult GetAll()
        {
            try
            {
                var result = _service.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi: " + ex.Message });
            }
        }

        /// <summary>
        /// Tìm kiếm phẫu thuật
        /// </summary>
        [HttpPost("search")]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<PagedResult<SurgeryScheduleDto>>> Search([FromBody] SearchRequestDTO request)
        {
            var res = _service.Search(request);
            return Ok(new ApiResponse<PagedResult<SurgeryScheduleDto>> { Success = true, Data = res, Message = "OK" });
        }

        /// <summary>
        /// Thêm lịch phẫu thuật mới
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<SurgeryScheduleDto>> Create(SurgeryScheduleDto dto)
        {
            var (userId, userName) = GetCurrentUser();
            var created = _service.Create(dto, userId, userName);
            if (created == null) 
                return BadRequest(new ApiResponse { Success = false, Message = "Create failed" });
            return Ok(new ApiResponse<SurgeryScheduleDto> { Success = true, Data = created, Message = "Created" });
        }

        /// <summary>
        /// Cập nhật lịch phẫu thuật
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<SurgeryScheduleDto>> Update(Guid id, SurgeryScheduleDto dto)
        {
            var (userId, userName) = GetCurrentUser();
            var updated = _service.Update(id, dto, userId, userName);
            if (updated == null) 
                return BadRequest(new ApiResponse { Success = false, Message = "Update failed" });
            return Ok(new ApiResponse<SurgeryScheduleDto> { Success = true, Data = updated, Message = "Updated" });
        }

        /// <summary>
        /// Xóa lịch phẫu thuật
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<ApiResponse> Delete(Guid id)
        {
            var (userId, userName) = GetCurrentUser();
            var ok = _service.Delete(id, userId, userName);
            return Ok(new ApiResponse { Success = ok, Message = ok ? "Deleted" : "Delete failed" });
        }
    }
}
