using Microsoft.AspNetCore.Mvc;
using YtaService.BLL.Interfaces;
using YtaService.DTO;

namespace YtaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiuongBenhController : ControllerBase
    {
        private readonly IGiuongBenhBusiness _business;

        public GiuongBenhController(IGiuongBenhBusiness business)
        {
            _business = business;
        }

        // 1. GET ALL
        // Viết gộp thế này Swagger sẽ hiểu ngay là: api/GiuongBenh/get-all
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var listGiuong = _business.GetAllGiuong();

            // Projection: Chỉ lấy dữ liệu cần thiết
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

        // 2. GET BY ID
        // Viết gộp: api/GiuongBenh/get-by-id/{id}
        [HttpGet("get-by-id/{id}")]
        public IActionResult GetById(Guid id)
        {
            var data = _business.GetById(id);
            if (data == null) return NotFound(new { Message = "Không tìm thấy giường này" });
            return Ok(data);
        }

        // 3. CREATE
        // Viết gộp: api/GiuongBenh/create
        [HttpPost("create")]
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
    }
}