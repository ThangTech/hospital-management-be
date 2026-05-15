using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
            private readonly IMedicalRecordReportService _reportService;

            public MedicalRecordController(IMedicalRecordService service, IMedicalRecordReportService reportService)
            {
                _service = service;
                _reportService = reportService;
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
                    return StatusCode(500, new ApiResponse { Success = false, Message = "Lỗi tải danh sách: " + ex.Message });
                }
            }

            /// <summary>
            /// Tìm kiếm hồ sơ bệnh án (theo tên bệnh nhân, tên bác sĩ, chẩn đoán...)
            /// </summary>
            [HttpPost("search")]
            [Authorize(Roles = "Admin,BacSi")]
            public ActionResult<ApiResponse<PagedResult<MedicalRecordDto>>> Search([FromBody] SearchRequestDTO request)
            {
                try
                {
                    var res = _service.GetByAdmission(
                        null,
                        request.PageNumber, request.PageSize, request.SearchTerm);

                    return Ok(new ApiResponse<PagedResult<MedicalRecordDto>>
                    {
                        Success = true,
                        Data = res,
                        Message = "OK"
                    });
                }
                catch (SqlException ex)
                {
                    return StatusCode(500, new ApiResponse { Success = false, Message = "Lỗi tìm kiếm: " + ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new ApiResponse { Success = false, Message = "Lỗi hệ thống: " + ex.Message });
                }
            }

            /// <summary>
            /// Thêm hồ sơ bệnh án mới
            /// Nghiệp vụ: 1 phiếu nhập viện chỉ được 1 hồ sơ, phiếu phải đang điều trị
            /// </summary>
            [HttpPost]
            [Authorize(Roles = "Admin,BacSi")]
            public ActionResult<ApiResponse<MedicalRecordDto>> Create(MedicalRecordDto dto)
            {
                try
                {
                    var (userId, userName) = GetCurrentUser();
                    var created = _service.Create(dto, userId, userName);

                    if (created == null)
                        return BadRequest(new ApiResponse { Success = false, Message = "Tạo hồ sơ thất bại" });

                    return Ok(new ApiResponse<MedicalRecordDto> { Success = true, Data = created, Message = "Tạo hồ sơ bệnh án thành công" });
                }
                catch (SqlException ex)
                {
                    // SP dùng RAISERROR → SqlException.Message chứa thông báo nghiệp vụ
                    return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new ApiResponse { Success = false, Message = "Lỗi hệ thống: " + ex.Message });
                }
            }

            /// <summary>
            /// Cập nhật hồ sơ bệnh án
            /// Nghiệp vụ: KetQuaDieuTri chỉ được cập nhật khi trạng thái "Chờ xuất viện"
            /// </summary>
            [HttpPut("{id:guid}")]
            [Authorize(Roles = "Admin,BacSi")]
            public ActionResult<ApiResponse<MedicalRecordDto>> Update(Guid id, MedicalRecordDto dto)
            {
                try
                {
                    var (userId, userName) = GetCurrentUser();
                    var updated = _service.Update(id, dto, userId, userName);

                    if (updated == null)
                        return BadRequest(new ApiResponse { Success = false, Message = "Cập nhật thất bại" });

                    return Ok(new ApiResponse<MedicalRecordDto> { Success = true, Data = updated, Message = "Cập nhật hồ sơ thành công" });
                }
                catch (SqlException ex)
                {
                    // SP dùng RAISERROR → SqlException.Message chứa thông báo nghiệp vụ
                    return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new ApiResponse { Success = false, Message = "Lỗi hệ thống: " + ex.Message });
                }
            }

            [HttpGet("export-pdf/{id:guid}")]
            [Authorize(Roles = "Admin,BacSi")]
            [Produces("application/pdf")]
            public IActionResult ExportPdf(Guid id)
            {
                var pdf = _reportService.ExportMedicalRecordPdf(id);
                if (pdf == null) return NotFound(new ApiResponse { Success = false, Message = "Không tìm thấy hồ sơ bệnh án" });
                return File(pdf, "application/pdf", $"HoSoBenhAn_{id}.pdf");
            }

            /// <summary>
            /// Xóa hồ sơ bệnh án
            /// </summary>
            [HttpDelete("{id:guid}")]
            [Authorize(Roles = "Admin")]
            public ActionResult<ApiResponse> Delete(Guid id)
            {
                try
                {
                    var (userId, userName) = GetCurrentUser();
                    var ok = _service.Delete(id, userId, userName);
                    return Ok(new ApiResponse { Success = ok, Message = ok ? "Đã xóa hồ sơ bệnh án" : "Xóa thất bại" });
                }
                catch (SqlException ex)
                {
                    return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new ApiResponse { Success = false, Message = "Lỗi hệ thống: " + ex.Message });
                }
            }
        }
    }
