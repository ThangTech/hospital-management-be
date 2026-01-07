using Microsoft.AspNetCore.Mvc;
using System;
using YtaService.BLL.Interfaces;
using YtaService.DTO;

namespace YtaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XuatVienController : ControllerBase
    {
        private readonly IXuatVienBusiness _bus;

        public XuatVienController(IXuatVienBusiness bus)
        {
            _bus = bus;
        }

        // PUT: api/XuatVien/process
        [HttpPut("xac-nhan")]
        public IActionResult XuatVien([FromBody] XuatVienDTO model)
        {
            try
            {
                var result = _bus.XuatVien(model);

                if (result == "Xuất viện thành công.")
                {
                    return Ok(new { message = result });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/XuatVien/ready-for-discharge
        [HttpGet("danh-sach-cho")]
        public IActionResult GetReadyForDischarge()
        {
            try
            {
                var data = _bus.LayDanhSachSanSangXuatVien();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/XuatVien/history
        [HttpGet("lich-su")]
        public IActionResult GetHistory()
        {
            try
            {
                var data = _bus.LayLichSuXuatVien();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/XuatVien/preview/{id}
        [HttpGet("kiem-tra-dieu-kien/{id}")]
        public IActionResult GetPreview(Guid id)
        {
            try
            {
                var data = _bus.XemTruocXuatVien(id);
                if (data == null)
                    return NotFound(new { message = "Không tìm thấy phiếu nhập viện." });
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
