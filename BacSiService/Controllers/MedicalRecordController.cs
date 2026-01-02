using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BacSiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _service;

        public MedicalRecordController(IMedicalRecordService service)
        {
            _service = service;
        }

        [HttpPost("search")]
        public ActionResult<ApiResponse<PagedResult<MedicalRecordDto>>> Search([FromBody] SearchRequestDTO request)
        {
            var res = _service.GetByAdmission(Guid.TryParse(request.SearchTerm, out var pid) ? pid : (Guid?)null,
                request.PageNumber, request.PageSize, request.SearchTerm);
            return Ok(new ApiResponse<PagedResult<MedicalRecordDto>> { Success = true, Data = res, Message = "OK" });
        }

        [HttpPost]
        public ActionResult<ApiResponse<MedicalRecordDto>> Create(MedicalRecordDto dto)
        {
            var created = _service.Create(dto);
            if (created == null) return BadRequest(new ApiResponse { Success = false, Message = "Create failed" });
            return Ok(new ApiResponse<MedicalRecordDto> { Success = true, Data = created, Message = "Created" });
        }

        [HttpPut("{id:guid}")]
        public ActionResult<ApiResponse<MedicalRecordDto>> Update(Guid id, MedicalRecordDto dto)
        {
            var updated = _service.Update(id, dto);
            if (updated == null) return BadRequest(new ApiResponse { Success = false, Message = "Update failed" });
            return Ok(new ApiResponse<MedicalRecordDto> { Success = true, Data = updated, Message = "Updated" });
        }

        [HttpDelete("{id:guid}")]
        public ActionResult<ApiResponse> Delete(Guid id)
        {
            var ok = _service.Delete(id);
            return Ok(new ApiResponse { Success = ok, Message = ok ? "Deleted" : "Delete failed" });
        }
    }
}
