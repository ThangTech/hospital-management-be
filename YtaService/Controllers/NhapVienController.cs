using System;
using Microsoft.AspNetCore.Mvc;
using YtaService.BLL.Interfaces;
using YtaService.DTO;

namespace YtaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhapVienController : ControllerBase
    {
        private readonly INhapVienBusiness _bus;

        public NhapVienController(INhapVienBusiness bus)
        {
            _bus = bus;
        }

        [HttpGet("danh-sach")]
        public IActionResult GetAll()
        {
            try
            {
                // SỬA THÀNH: LayDanhSachNoiTru (Cho khớp với Interface và Business)
                var data = _bus.LayDanhSachNoiTru();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("chi-tiet/{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var data = _bus.LayChiTietNhapVien(id);
                if (data == null)
                    return NotFound(new { message = "Không tìm thấy phiếu nhập viện." });
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        // 2. CREATE (NHẬP VIỆN)
        [HttpPost("nhap-vien-moi")]
        public IActionResult Create([FromBody] NhapVienCreateDTO model)
        {
            try
            {
                bool result = _bus.NhapVienMoi(model);
                if (result)
                    return Ok(new { message = "Nhập viện thành công." });
                else
                    return BadRequest(new { message = "Nhập viện thất bại (Giường bận hoặc lỗi hệ thống)." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 3. UPDATE (CẬP NHẬT NHẬP VIỆN)
        [HttpPut("cap-nhat")]
        public IActionResult Update([FromBody] NhapVienUpdateDTO model)
        {
            try
            {
                var result = _bus.CapNhatNhapVien(model);

                if (result == "Cập nhật thành công.")
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

        // 4. DELETE (XÓA NHẬP VIỆN)
        [HttpDelete("xoa/{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                var result = _bus.XoaNhapVien(id);

                if (result == "Xóa thành công.")
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

        // 5. CHUYỂN GIƯỜNG
        [HttpPut("chuyen-giuong")]
        public IActionResult ChuyenGiuong([FromBody] ChuyenGiuongDTO model)
        {
            try
            {
                var result = _bus.ChuyenGiuong(model);

                if (result == "Chuyển giường thành công.")
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

        // 6. TÌM KIẾM NHẬP VIỆN
        [HttpPost("tim-kiem")]
        public IActionResult Search([FromBody] NhapVienSearchDTO model)
        {
            try
            {
                var data = _bus.TimKiem(model);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
