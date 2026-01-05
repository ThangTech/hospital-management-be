using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YtaService.BLL.Interfaces;
using YtaService.DTO;

namespace YtaService.Controllers
{
    /// <summary>
    /// Controller quản lý Giường bệnh
    /// Quyền: Admin, YTa
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập
    public class GiuongBenhController : ControllerBase
    {
        private readonly IGiuongBenhBusiness _business;

        public GiuongBenhController(IGiuongBenhBusiness business)
        {
            _business = business;
        }

        /// <summary>
        /// Lấy tất cả giường bệnh
        /// Quyền: Admin, YTa
        /// </summary>
        [HttpGet("get-all")]
        [Authorize(Roles = "Admin,YTa")]
        public IActionResult GetAll()
        {
            var listGiuong = _business.GetAllGiuong();

            var result = listGiuong.Select(g => new
            {
                id = g.Id,
                khoaId = g.KhoaId,
                tenGiuong = g.TenGiuong,
                loaiGiuong = g.LoaiGiuong,
                giaTien = g.GiaTien,
                trangThai = g.TrangThai
            });

            return Ok(result);
        }

        /// <summary>
        /// Lấy giường bệnh theo ID
        /// Quyền: Admin, YTa
        /// </summary>
        [HttpGet("get-by-id/{id}")]
        [Authorize(Roles = "Admin,YTa")]
        public IActionResult GetById(Guid id)
        {
            var data = _business.GetById(id);
            if (data == null) return NotFound(new { Message = "Không tìm thấy giường này" });
            return Ok(data);
        }

        /// <summary>
        /// Thêm giường bệnh mới
        /// Quyền: Admin, YTa
        /// </summary>
        [HttpPost("create")]
        [Authorize(Roles = "Admin,YTa")]
        public IActionResult Create([FromBody] GiuongBenhCreateDTO dto)
        {
            try
            {
                _business.CreateGiuong(dto);
                return Ok(new { Message = "Thêm giường thành công" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật giường bệnh
        /// Quyền: Admin, YTa
        /// </summary>
        [HttpPut("update-giuong")]
        [Authorize(Roles = "Admin,YTa")]
        public IActionResult UpdateGiuong([FromBody] GiuongUpdateDTO request)
        {
            try
            {
                var result = _business.UpdateGiuong(request);

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

        /// <summary>
        /// Xóa giường bệnh
        /// Quyền: Chỉ Admin
        /// </summary>
        [HttpDelete("delete-giuong/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteGiuong(Guid id)
        {
            try
            {
                var result = _business.DeleteGiuong(id);

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
    }
}