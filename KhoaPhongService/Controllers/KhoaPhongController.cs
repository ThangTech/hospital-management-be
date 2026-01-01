using KhoaPhongService.BLL.Interfaces;
using KhoaPhongService.DTO;
using KhoaPhongService.Models;
using Microsoft.AspNetCore.Mvc;

namespace KhoaPhongService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhoaPhongController : ControllerBase
    {
        private readonly IKhoaPhongBusiness _bus;

        public KhoaPhongController(IKhoaPhongBusiness bus)
        {
            _bus = bus;
        }

        // 1. L?y t?t c?
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var data = _bus.GetAll();

            var viewData = data.Select(x => new KhoaPhongViewDTO
            {
                Id = x.Id,
                TenKhoa = x.TenKhoa,
                LoaiKhoa = x.LoaiKhoa,
                // [S?A L?I]: Thêm ?? 0 vào ?ây
                SoGiuongTieuChuan = x.SoGiuongTieuChuan ?? 0
            }).ToList();

            return Ok(viewData);
        }

        // 2. L?y chi ti?t
        [HttpGet("get-by-id/{id}")]
        public IActionResult GetById(string id)
        {
            var x = _bus.GetById(id);
            if (x == null) return NotFound("Không tìm th?y");

            var viewDto = new KhoaPhongViewDTO
            {
                Id = x.Id,
                TenKhoa = x.TenKhoa,
                LoaiKhoa = x.LoaiKhoa,
                // [S?A L?I]: Thêm ?? 0 vào ?ây n?a
                SoGiuongTieuChuan = x.SoGiuongTieuChuan ?? 0
            };
            return Ok(viewDto);
        }

        // 3. Thêm m?i
        [HttpPost("create")]
        public IActionResult Create([FromBody] KhoaPhongCreateDTO model)
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
                return Ok(new { Msg = "Thêm thành công", Data = entity });
            }
            return BadRequest("Thêm th?t b?i");
        }
    }
}