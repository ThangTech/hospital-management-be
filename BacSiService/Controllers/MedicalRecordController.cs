using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace BacSiService.Controllers
{
    /// <summary>
    /// Controller quản lý Hồ sơ bệnh án
    /// Quyền: Admin, BacSi
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _service;

        public MedicalRecordController(IMedicalRecordService service)
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
        /// Lấy tất cả hồ sơ bệnh án
        /// </summary>
        [HttpGet("get-all-medical")]
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
        /// Tìm kiếm hồ sơ bệnh án
        /// </summary>
        [HttpPost("search")]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<PagedResult<MedicalRecordDto>>> Search([FromBody] SearchRequestDTO request)
        {
            var res = _service.GetByAdmission(
                Guid.TryParse(request.SearchTerm, out var pid) ? pid : (Guid?)null,
                request.PageNumber, request.PageSize, request.SearchTerm);
            return Ok(new ApiResponse<PagedResult<MedicalRecordDto>> { Success = true, Data = res, Message = "OK" });
        }

        /// <summary>
        /// Thêm hồ sơ bệnh án mới
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<MedicalRecordDto>> Create(MedicalRecordDto dto)
        {
            var (userId, userName) = GetCurrentUser();
            var created = _service.Create(dto, userId, userName);
            if (created == null) 
                return BadRequest(new ApiResponse { Success = false, Message = "Create failed" });
            return Ok(new ApiResponse<MedicalRecordDto> { Success = true, Data = created, Message = "Created" });
        }

        /// <summary>
        /// Cập nhật hồ sơ bệnh án
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<MedicalRecordDto>> Update(Guid id, MedicalRecordDto dto)
        {
            var (userId, userName) = GetCurrentUser();
            var updated = _service.Update(id, dto, userId, userName);
            if (updated == null) 
                return BadRequest(new ApiResponse { Success = false, Message = "Update failed" });
            return Ok(new ApiResponse<MedicalRecordDto> { Success = true, Data = updated, Message = "Updated" });
        }

        /// <summary>
        /// Xóa hồ sơ bệnh án
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
