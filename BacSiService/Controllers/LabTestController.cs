using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace BacSiService.Controllers
{
    /// <summary>
    /// Controller quản lý Xét nghiệm
    /// Quyền: Admin, BacSi
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LabTestController : ControllerBase
    {
        private readonly ILabTestService _service;

        public LabTestController(ILabTestService service)
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
        /// Lấy tất cả xét nghiệm
        /// </summary>
        [HttpGet("get-all-labtest")]
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
        /// Tìm kiếm xét nghiệm
        /// </summary>
        [HttpPost("search")]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<PagedResult<LabTestDto>>> Search([FromBody] SearchRequestDTO request)
        {
            var res = _service.Search(request);
            return Ok(new ApiResponse<PagedResult<LabTestDto>> { Success = true, Data = res, Message = "OK" });
        }

        /// <summary>
        /// Thêm xét nghiệm mới
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<LabTestDto>> Create(LabTestDto dto)
        {
            var (userId, userName) = GetCurrentUser();
            var created = _service.Create(dto, userId, userName);
            if (created == null) 
                return BadRequest(new ApiResponse { Success = false, Message = "Create failed" });
            return Ok(new ApiResponse<LabTestDto> { Success = true, Data = created, Message = "Created" });
        }

        /// <summary>
        /// Cập nhật xét nghiệm
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<LabTestDto>> Update(Guid id, LabTestDto dto)
        {
            var (userId, userName) = GetCurrentUser();
            var updated = _service.Update(id, dto, userId, userName);
            if (updated == null) 
                return BadRequest(new ApiResponse { Success = false, Message = "Update failed" });
            return Ok(new ApiResponse<LabTestDto> { Success = true, Data = updated, Message = "Updated" });
        }

        /// <summary>
        /// Xóa xét nghiệm
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
