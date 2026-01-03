using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BacSiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabTestController : ControllerBase
    {
        private readonly ILabTestService _service;

        public LabTestController(ILabTestService service)
        {
            _service = service;
        }
        [HttpGet("get-all-labtest")]
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

        [HttpPost("search")]
        public ActionResult<ApiResponse<PagedResult<LabTestDto>>> Search([FromBody] SearchRequestDTO request)
        {
            var res = _service.Search(request);
            return Ok(new ApiResponse<PagedResult<LabTestDto>> { Success = true, Data = res, Message = "OK" });
        }

        [HttpPost]
        public ActionResult<ApiResponse<LabTestDto>> Create(LabTestDto dto)
        {
            var created = _service.Create(dto);
            if (created == null) return BadRequest(new ApiResponse { Success = false, Message = "Create failed" });
            return Ok(new ApiResponse<LabTestDto> { Success = true, Data = created, Message = "Created" });
        }

        [HttpPut("{id:guid}")]
        public ActionResult<ApiResponse<LabTestDto>> Update(Guid id, LabTestDto dto)
        {
            var updated = _service.Update(id, dto);
            if (updated == null) return BadRequest(new ApiResponse { Success = false, Message = "Update failed" });
            return Ok(new ApiResponse<LabTestDto> { Success = true, Data = updated, Message = "Updated" });
        }

        [HttpDelete("{id:guid}")]
        public ActionResult<ApiResponse> Delete(Guid id)
        {
            var ok = _service.Delete(id);
            return Ok(new ApiResponse { Success = ok, Message = ok ? "Deleted" : "Delete failed" });
        }
    }
}
