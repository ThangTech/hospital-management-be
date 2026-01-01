using Microsoft.AspNetCore.Mvc;
using YtaService.BLL.Interfaces;
using YtaService.DTO;
using YtaService.Models;

namespace YtaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YtaController : ControllerBase
    {
        private readonly IYtaBusiness _business;

        public YtaController(IYtaBusiness business)
        {
            _business = business;
        }

        // 1. Search
        [Route("search")]
        [HttpPost]
        public IActionResult Search([FromBody] YTaSearchDTO model)
        {
            try
            {
                long total = 0;
                var data = _business.Search(model, out total);

                // Map Entity -> ViewDTO
                var viewData = data.Select(x => new YTaViewDTO
                {
                    Id = x.Id,
                    HoTen = x.HoTen,
                    NgaySinh = x.NgaySinh,
                    GioiTinh = x.GioiTinh,
                    SoDienThoai = x.SoDienThoai,
                    KhoaId = x.KhoaId,
                    ChungChiHanhNghe = x.ChungChiHanhNghe
                }).ToList();

                var result = new PagedResult<YTaViewDTO>
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    TotalRecords = total,
                    Items = viewData
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 2. Create
        [Route("create")]
        [HttpPost]
        public IActionResult Create([FromBody] YtaCreateDTO model)
        {
            try
            {
                var entity = new YTa
                {
                    Id = Guid.NewGuid(),
                    HoTen = model.HoTen,
                    NgaySinh = model.NgaySinh,
                    GioiTinh = model.GioiTinh,
                    SoDienThoai = model.SoDienThoai,
                    KhoaId = model.KhoaId,
                    ChungChiHanhNghe = model.ChungChiHanhNghe
                };

                if (_business.Create(entity))
                {
                    return Ok(new { Message = "Thêm Y tá thành công", Data = entity });
                }
                return BadRequest("Thêm thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 6. Get All
        [Route("get-all")]
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var data = _business.GetAll();

                // Map sang ViewDTO để ẩn các thông tin không cần thiết (nếu có)
                var viewData = data.Select(x => new YTaViewDTO
                {
                    Id = x.Id,
                    HoTen = x.HoTen,
                    NgaySinh = x.NgaySinh,
                    GioiTinh = x.GioiTinh,
                    SoDienThoai = x.SoDienThoai,
                    KhoaId = x.KhoaId,
                    ChungChiHanhNghe = x.ChungChiHanhNghe
                }).ToList();

                return Ok(viewData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 3. Update
        [Route("update")]
        [HttpPut]
        // [SỬA LỖI]: Đổi YTaUpdateDTO thành YtaUpdateDTO (chữ t thường) để khớp với file DTO
        public IActionResult Update([FromBody] YTaUpdateDTO model)
        {
            try
            {
                var entity = new YTa
                {
                    Id = model.Id,
                    HoTen = model.HoTen,
                    NgaySinh = model.NgaySinh,
                    GioiTinh = model.GioiTinh,
                    SoDienThoai = model.SoDienThoai,
                    KhoaId = model.KhoaId,
                    ChungChiHanhNghe = model.ChungChiHanhNghe
                };

                if (_business.Update(entity))
                {
                    return Ok(new { Message = "Cập nhật thành công" });
                }
                return BadRequest("Cập nhật thất bại hoặc không tìm thấy ID");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 4. Delete
        [Route("delete/{id}")]
        [HttpDelete]
        public IActionResult Delete(string id)
        {
            if (_business.Delete(id)) return Ok(new { Message = "Xóa thành công" });
            return BadRequest("Xóa thất bại");
        }

        // 5. GetById
        [Route("get-by-id/{id}")]
        [HttpGet]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);
            if (data == null) return NotFound("Không tìm thấy");

            // Map sang ViewDTO
            var viewDto = new YTaViewDTO
            {
                Id = data.Id,
                HoTen = data.HoTen,
                NgaySinh = data.NgaySinh,
                GioiTinh = data.GioiTinh,
                SoDienThoai = data.SoDienThoai,
                KhoaId = data.KhoaId,
                ChungChiHanhNghe = data.ChungChiHanhNghe
            };
            return Ok(viewDto);
        }
    }
}