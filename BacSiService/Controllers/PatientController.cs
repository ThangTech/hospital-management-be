using BacSiService.BLL.Interfaces;
using BacSiService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BacSiService.Controllers
{
    /// <summary>
    /// Controller tra cứu bệnh nhân
    /// Quyền: Admin, BacSi
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientController(IPatientService service)
        {
            _service = service;
        }

        /// <summary>
        /// Tra cứu bệnh nhân
        /// Quyền: Admin, BacSi
        /// </summary>
        [HttpPost("lookup")]
        [Authorize(Roles = "Admin,BacSi")]
        public ActionResult<ApiResponse<PagedResult<PatientLookupDto>>> Lookup([FromBody] SearchRequestDTO request)
        {
            var res = _service.Lookup(request.SearchTerm, request.PageNumber, request.PageSize);
            return Ok(new ApiResponse<PagedResult<PatientLookupDto>> { Success = true, Data = res, Message = "OK" });
        }
    }
}
