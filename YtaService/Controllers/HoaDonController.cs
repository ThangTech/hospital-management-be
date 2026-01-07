using Microsoft.AspNetCore.Mvc;
using System;
using YtaService.BLL.Interfaces;
using YtaService.DTO;

namespace YtaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly IHoaDonBusiness _bus;

        public HoaDonController(IHoaDonBusiness bus)
        {
            _bus = bus;
        }

        [HttpGet("xem-truoc/{nhapVienId}")]
        public IActionResult GetPreview(Guid nhapVienId)
        {
            var data = _bus.LayPreviewGoiY(nhapVienId);
            if (data == null) return NotFound(new { message = "Không tìm thấy thông tin nhập viện để gợi ý." });
            return Ok(data);
        }

        [HttpPost("tao-moi")]
        public IActionResult Create([FromBody] HoaDonCreateDTO model)
        {
            var result = _bus.TaoHoaDonMoi(model);
            if (result == "Tạo hóa đơn thành công.") return Ok(new { message = result });
            return BadRequest(new { message = result });
        }

        [HttpGet("lay-tat-ca")]
        public IActionResult GetAll()
        {
            var data = _bus.LayToanBoHoaDon();
            return Ok(data);
        }

        [HttpGet("danh-sach")]
        public IActionResult GetList(Guid? benhNhanId, Guid? nhapVienId)
        {
            var data = _bus.LayDanhSachHoaDon(benhNhanId, nhapVienId);
            return Ok(data);
        }

        [HttpGet("chi-tiet/{id}")]
        public IActionResult GetById(Guid id)
        {
            var data = _bus.LayChiTietHoaDon(id);
            if (data == null) return NotFound(new { message = "Không tìm thấy hóa đơn." });
            return Ok(data);
        }

        [HttpPut("thanh-toan")]
        public IActionResult Payment([FromBody] HoaDonThanhToanDTO model)
        {
            var result = _bus.ThanhToanHoaDon(model);
            if (result == "Thanh toán thành công.") return Ok(new { message = result });
            return BadRequest(new { message = result });
        }

        [HttpDelete("xoa/{id}")]
        public IActionResult Delete(Guid id)
        {
            var result = _bus.XoaHoaDon(id);
            if (result == "Xóa hóa đơn thành công.") return Ok(new { message = result });
            return BadRequest(new { message = result });
        }
    }
}
