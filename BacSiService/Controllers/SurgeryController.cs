using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BacSiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurgeryController : ControllerBase
    {
        private readonly ISurgeryService _service;

        public SurgeryController(ISurgeryService service)
        {
            _service = service;
        }

        [HttpPost("search")]
        public ActionResult<ApiResponse<PagedResult<SurgeryScheduleDto>>> Search([FromBody] SearchRequestDTO request)
        {
            var res = _service.Search(request);
            return Ok(new ApiResponse<PagedResult<SurgeryScheduleDto>> { Success = true, Data = res, Message = "OK" });
        }

        [HttpPost]
        public ActionResult<ApiResponse<SurgeryScheduleDto>> Create(SurgeryScheduleDto dto)
        {
            var created = _service.Create(dto);
            if (created == null) return BadRequest(new ApiResponse { Success = false, Message = "Create failed" });
            return Ok(new ApiResponse<SurgeryScheduleDto> { Success = true, Data = created, Message = "Created" });
        }

        [HttpPut("{id:guid}")]
        public ActionResult<ApiResponse<SurgeryScheduleDto>> Update(Guid id, SurgeryScheduleDto dto)
        {
            var updated = _service.Update(id, dto);
            if (updated == null) return BadRequest(new ApiResponse { Success = false, Message = "Update failed" });
            return Ok(new ApiResponse<SurgeryScheduleDto> { Success = true, Data = updated, Message = "Updated" });
        }

        [HttpDelete("{id:guid}")]
        public ActionResult<ApiResponse> Delete(Guid id)
        {
            var ok = _service.Delete(id);
            return Ok(new ApiResponse { Success = ok, Message = ok ? "Deleted" : "Delete failed" });
        }
    }
}
