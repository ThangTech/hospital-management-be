using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using YtaService.BLL.Interfaces;
using YtaService.DTO;

namespace YtaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,KeToan,BenhNhan")]
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
            if (data == null) return NotFound(new { message = "Kh�ng t�m th?y th�ng tin nh?p vi?n d? g?i �." });
            return Ok(data);
        }

        [HttpPost("tao-moi")]
        public IActionResult Create([FromBody] HoaDonCreateDTO model)
        {
            var result = _bus.TaoHoaDonMoi(model);
            if (result == "T?o h�a don th�nh c�ng.") return Ok(new { message = result });
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
            if (data == null) return NotFound(new { message = "Kh�ng t�m th?y h�a don." });
            return Ok(data);
        }

        [HttpPut("thanh-toan")]
        public IActionResult Payment([FromBody] HoaDonThanhToanDTO model)
        {
            var result = _bus.ThanhToanHoaDon(model);
            if (result == "Thanh to�n th�nh c�ng.") return Ok(new { message = result });
            return BadRequest(new { message = result });
        }

        [HttpDelete("xoa/{id}")]
        public IActionResult Delete(Guid id)
        {
            var result = _bus.XoaHoaDon(id);
            if (result == "X�a h�a don th�nh c�ng.") return Ok(new { message = result });
            return BadRequest(new { message = result });
        }

        [HttpGet("export-pdf/{id}")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ExportPdf(Guid id)
        {
            var pdf = _reportBus.ExportHoaDonPdf(id);
            if (pdf == null) return NotFound(new { message = "Kh�ng t�m th?y h�a don d? xu?t PDF." });
            
            // �?m b?o tr�nh duy?t nh?n di?n l� file t?i v?
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
            if (excel == null) return NotFound(new { message = "Kh�ng t�m th?y h�a don d? xu?t Excel." });
            
            // �?m b?o tr�nh duy?t nh?n di?n l� file t?i v? v?i Content-Type chu?n
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"HoaDon_{id}.xlsx");
        }

        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new { message = "Vui l�ng ch?n file Excel." });
            
            using (var ms = new System.IO.MemoryStream())
            {
                await file.CopyToAsync(ms);
                var result = await _reportBus.ImportHoaDonFromExcel(ms.ToArray());
                return Ok(new { message = result });
            }
        }
    }
}
