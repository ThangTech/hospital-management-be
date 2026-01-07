using Microsoft.AspNetCore.Mvc;
using System;
using YtaService.BLL; // Using namespace chứa Interface Business
using YtaService.DTO;
using YtaService.BLL.Interfaces;

namespace YtaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoSoBenhAnController : ControllerBase
    {
        private readonly IHoSoBenhAnBusiness _bus;

        public HoSoBenhAnController(IHoSoBenhAnBusiness bus)
        {
            _bus = bus;
        }

        // POST: api/HoSoBenhAn
        [HttpPost("tao-moi")]
        public IActionResult Create([FromBody] HoSoBenhAnCreateDTO model)
        {
            if (model == null) return BadRequest("Dữ liệu không hợp lệ");

            var result = _bus.TaoMoi(model);
            if (result)
                return Ok(new { message = "Tạo hồ sơ bệnh án thành công" });

            return BadRequest(new { message = "Tạo thất bại" });
        }
        [HttpGet("danh-sach")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _bus.LayTatCaHoSo();
                // Trả về 200 OK cùng dữ liệu
                return Ok(data);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // GET: api/HoSoBenhAn/ByNhapVien/{guid}
        [HttpGet("theo-nhap-vien/{nhapVienId}")]
        public IActionResult GetByNhapVien(Guid nhapVienId)
        {
            var data = _bus.LayTheoNhapVien(nhapVienId);
            return Ok(data);
        }
    }
}