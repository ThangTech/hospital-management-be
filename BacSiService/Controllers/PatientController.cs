using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BacSiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientController(IPatientService service)
        {
            _service = service;
        }

        [HttpPost("lookup")]
        public ActionResult<ApiResponse<PagedResult<PatientLookupDto>>> Lookup([FromBody] SearchRequestDTO request)
        {
            var res = _service.Lookup(request.SearchTerm, request.PageNumber, request.PageSize);
            return Ok(new ApiResponse<PagedResult<PatientLookupDto>> { Success = true, Data = res, Message = "OK" });
        }
    }
}
