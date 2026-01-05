using KhoaPhongService.BLL.Interfaces;
using KhoaPhongService.DTO;
using KhoaPhongService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace KhoaPhongService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập cho tất cả endpoints
    public class KhoaPhongController : ControllerBase
    {
        private readonly IKhoaPhongBusiness _bus;

        public KhoaPhongController(IKhoaPhongBusiness bus)
        {
            _bus = bus;
        }

        /// <summary>
        /// Lấy tất cả khoa phòng
        /// Quyền: Admin, BacSi, YTa
        /// </summary>
        [HttpGet("get-all")]
        [Authorize(Roles = "Admin,BacSi,YTa")]
        public IActionResult GetAll()
        {
            var data = _bus.GetAll();

            var viewData = data.Select(x => new KhoaPhongViewDTO
            {
                Id = x.Id,
                TenKhoa = x.TenKhoa,
                LoaiKhoa = x.LoaiKhoa,
                SoGiuongTieuChuan = x.SoGiuongTieuChuan ?? 0
            }).ToList();

            return Ok(viewData);
        }

        /// <summary>
        /// Lấy khoa phòng theo ID
        /// Quyền: Admin, BacSi, YTa
        /// </summary>
        [HttpGet("get-by-id/{id}")]
        [Authorize(Roles = "Admin,BacSi,YTa")]
        public IActionResult GetById(string id)
        {
            var x = _bus.GetById(id);
            if (x == null) return NotFound("Không tìm thấy");

            var viewDto = new KhoaPhongViewDTO
            {
                Id = x.Id,
                TenKhoa = x.TenKhoa,
                LoaiKhoa = x.LoaiKhoa,
                SoGiuongTieuChuan = x.SoGiuongTieuChuan ?? 0
            };
            return Ok(viewDto);
        }

        /// <summary>
        /// Thêm khoa phòng mới
        /// Quyền: Admin, YTa
        /// </summary>
        [HttpPost("create")]
        [Authorize(Roles = "Admin,YTa")]
        public IActionResult Create([FromBody] KhoaPhongCreateDTO model)
        {
            try
            {
                var entity = new KhoaPhong
                {
                    Id = Guid.NewGuid(), 
                    TenKhoa = model.TenKhoa,
                    LoaiKhoa = model.LoaiKhoa,
                    SoGiuongTieuChuan = model.SoGiuongTieuChuan
                };

                if (_bus.Create(entity))
                {
                    return Ok(new { Msg = "Thêm khoa mới thành công", Data = entity });
                }

                return BadRequest(new { Msg = "Thêm thất bại do lỗi không xác định" });
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Number == 50001)
                {
                    return BadRequest(new { Msg = ex.Message }); 
                }

                return StatusCode(500, new { Msg = "Lỗi Database: " + ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Msg = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật khoa phòng
        /// Quyền: Admin, YTa
        /// </summary>
        [HttpPut("update")]
        [Authorize(Roles = "Admin,YTa")]
        public IActionResult Update([FromBody] KhoaPhongUpdateDTO model)
        {
            try
            {
                var entity = new KhoaPhong
                {
                    Id = model.Id,
                    TenKhoa = model.TenKhoa,
                    LoaiKhoa = model.LoaiKhoa,
                    SoGiuongTieuChuan = model.SoGiuongTieuChuan
                };

                if (_bus.Update(entity))
                {
                    return Ok(new { Msg = "Cập nhật thành công" });
                }

                return BadRequest(new { Msg = "Cập nhật thất bại (ID không tồn tại)" });
            }
            catch (SqlException ex) 
            {
                if (ex.Number == 50002)
                {
                    return BadRequest(new { Msg = ex.Message });
                }
                return StatusCode(500, new { Msg = "Lỗi Database: " + ex.Message });
            }
        }

        /// <summary>
        /// Tìm kiếm khoa phòng
        /// Quyền: Admin, BacSi, YTa
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "Admin,BacSi,YTa")]
        public IActionResult Search([FromQuery] string keyword)
        {
            try
            {
                var result = _bus.Search(keyword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Msg = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// <summary>
        /// Xóa khoa phòng
        /// Quyền: Chỉ Admin
        /// </summary>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            try
            {
                if (_bus.Delete(id))
                {
                    return Ok(new { Msg = "Xóa thành công" });
                }
                return BadRequest(new { Msg = "Xóa thất bại" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Msg = ex.Message });
            }
        }
    }
}