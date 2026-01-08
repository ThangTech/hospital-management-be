using BenhNhanService.BLL.Interfaces;
using BenhNhanService.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BenhNhanService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BHYTController : ControllerBase
    {
        private readonly IBHYTBusiness _bhytBus;

        public BHYTController(IBHYTBusiness bhytBus)
        {
            _bhytBus = bhytBus;
        }

        [HttpGet("kiem-tra-the-bhyt/{soThe}")]
        [Authorize(Roles = "Admin,YTa,BacSi")]
        public IActionResult KiemTraThe(string soThe)
        {
            var result = _bhytBus.CheckValidity(soThe);
            return Ok(result);
        }

        [HttpPost("tinh-toan-chi-phi-bhyt")]
        [Authorize(Roles = "Admin,YTa,KeToan")]
        public IActionResult TinhToanChiPhi([FromBody] YeuCauTinhPhiBHYT request)
        {
            var result = _bhytBus.CalculatePayout(request);
            if (result == null) return NotFound("Không tìm thấy bệnh nhân");
            return Ok(result);
        }
    }
}
