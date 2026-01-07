using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using YtaService.BLL.Interfaces;
using YtaService.DTO;

namespace YtaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,KeToan")]
    public class HoaDonController : ControllerBase
    {
        private readonly IHoaDonBusiness _bus;
        private readonly IHoaDonReportBusiness _reportBus;

        public HoaDonController(IHoaDonBusiness bus, IHoaDonReportBusiness reportBus)
        {
            _bus = bus;
            _reportBus = reportBus;
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

        [HttpGet("export-pdf/{id}")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ExportPdf(Guid id)
        {
            var pdf = _reportBus.ExportHoaDonPdf(id);
            if (pdf == null) return NotFound(new { message = "Không tìm thấy hóa đơn để xuất PDF." });
            
            // Đảm bảo trình duyệt nhận diện là file tải về
            return File(pdf, "application/pdf", $"HoaDon_{id}.pdf");
        }

        [HttpGet("export-excel")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        public IActionResult ExportExcel(Guid? benhNhanId, Guid? nhapVienId)
        {
            var excel = _reportBus.ExportDanhSachHoaDonExcel(benhNhanId, nhapVienId);
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DanhSachHoaDon_{DateTime.Now:yyyyMMdd}.xlsx");
        }


        [HttpGet("export-excel/{id}")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ExportExcel(Guid id)
        {
            var excel = _reportBus.ExportHoaDonExcel(id);
            if (excel == null) return NotFound(new { message = "Không tìm thấy hóa đơn để xuất Excel." });
            
            // Đảm bảo trình duyệt nhận diện là file tải về với Content-Type chuẩn
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"HoaDon_{id}.xlsx");
        }

        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new { message = "Vui lòng chọn file Excel." });
            
            using (var ms = new System.IO.MemoryStream())
            {
                await file.CopyToAsync(ms);
                var result = await _reportBus.ImportHoaDonFromExcel(ms.ToArray());
                return Ok(new { message = result });
            }
        }
    }
}
