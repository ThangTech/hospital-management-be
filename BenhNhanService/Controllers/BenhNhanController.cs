using BenhNhanService.BLL.Interfaces;
using QuanLyBenhNhan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BenhNhanService.DTO;

namespace BenhNhanService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BenhNhanController : ControllerBase
    {
        private IBenhNhanBusiness _benhNhanBusiness;
        public BenhNhanController(IBenhNhanBusiness bus) { _benhNhanBusiness = bus; }

        // --- 1. CREATE: Thêm bệnh nhân mới ---
        [HttpPost("create")]
        [Authorize(Roles = "Admin,YTa")]
        public IActionResult CreateItem([FromBody] BenhNhanCreateDTO modelDto)
        {
            try
            {
                var benhNhan = new BenhNhan
                {
                    Id = Guid.NewGuid(),
                    HoTen = modelDto.HoTen,
                    NgaySinh = modelDto.NgaySinh,
                    GioiTinh = modelDto.GioiTinh,
                    DiaChi = modelDto.DiaChi,
                    SoTheBaoHiem = modelDto.SoTheBaoHiem,
                    MucHuong = modelDto.MucHuong,
                    HanTheBHYT = modelDto.HanTheBHYT,
                    TrangThai = modelDto.TrangThai ?? "Đang điều trị"
                };

                _benhNhanBusiness.Create(benhNhan);

                // CHUYỂN ĐỔI NGƯỢC LẠI SANG VIEW DTO ĐỂ TRẢ VỀ
                var resultDTO = MapToViewDTO(benhNhan);

                return StatusCode(201, new { Message = "Thêm thành công", Data = resultDTO });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // --- 2. UPDATE: Cập nhật thông tin bệnh nhân ---
        [HttpPut("update")]
        [Authorize(Roles = "Admin,YTa")]
        public IActionResult UpdateItem([FromBody] BenhNhanUpdateDTO modelDto)
        {
            try
            {
                // Bước 1: Kiểm tra tồn tại
                var existingItem = _benhNhanBusiness.GetDatabyID(modelDto.Id.ToString());
                if (existingItem == null) return NotFound("Không tìm thấy bệnh nhân để sửa");

                // Bước 2: Cập nhật dữ liệu vào Entity
                var benhNhan = new BenhNhan
                {
                    Id = modelDto.Id, // Giữ nguyên ID cũ
                    HoTen = modelDto.HoTen,
                    NgaySinh = modelDto.NgaySinh,
                    GioiTinh = modelDto.GioiTinh,
                    DiaChi = modelDto.DiaChi,
                    SoTheBaoHiem = modelDto.SoTheBaoHiem,
                    MucHuong = modelDto.MucHuong,
                    HanTheBHYT = modelDto.HanTheBHYT,
                    TrangThai = modelDto.TrangThai ?? "Đang điều trị"
                };

                _benhNhanBusiness.Update(benhNhan);

                // CHUYỂN ĐỔI SANG VIEW DTO
                var resultDTO = MapToViewDTO(benhNhan);

                return Ok(new { Message = "Cập nhật thành công", Data = resultDTO });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // --- 3. DELETE: Xóa bệnh nhân theo ID (có kiểm tra ràng buộc) ---
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBenhNhan(string id)
        {
            try
            {

                // Bước 1: Lấy thông tin bệnh nhân TRƯỚC khi xóa
                var itemToDelete = _benhNhanBusiness.GetDatabyID(id);
                if (itemToDelete == null) return NotFound("Không tìm thấy bệnh nhân để xóa");

                // Bước 2: Thực hiện xóa
                _benhNhanBusiness.Delete(id);

                // Bước 3: Chuyển đổi thông tin vừa xóa sang DTO để hiển thị
                var resultDTO = MapToViewDTO(itemToDelete);

                return Ok(new { Message = "Đã xóa thành công bệnh nhân sau", Data = resultDTO });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // --- 4. GET BY ID: Trả về ViewDTO ---
        [HttpGet("get-by-id/{id}")]
        [Authorize(Roles = "Admin,YTa,BacSi,KeToan")]
        public IActionResult GetDatabyID(string id)
        {
            var data = _benhNhanBusiness.GetDatabyID(id);
            if (data == null) return NotFound("Không tìm thấy");

            return Ok(MapToViewDTO(data));
        }

        // --- 5. GET ALL: Trả về List<ViewDTO> ---
        [HttpGet("get-all")]
        [Authorize(Roles = "Admin,YTa,BacSi,KeToan")]
        public IActionResult GetAll()
        {
            var listEntity = _benhNhanBusiness.GetAll();
            // Dùng hàm Map cho cả danh sách
            var listView = listEntity.Select(x => MapToViewDTO(x)).ToList();
            return Ok(listView);
        }

        [HttpPost("search")]
        [Authorize(Roles = "Admin,YTa,BacSi,KeToan")]
        public IActionResult Search([FromBody] BenhNhanSearchDTO.BenhNhanSearchModel searchModel)
        {
            try
            {
                // 1. Gọi Business tìm kiếm
                long totalRecords = 0;
                var listEntity = _benhNhanBusiness.Search(searchModel, out totalRecords);

                // 2. Map sang ViewDTO
                var listView = listEntity.Select(x => MapToViewDTO(x)).ToList();

                // 3. Trả về kết quả phân trang
                var result = new BenhNhanSearchDTO.PagedResult<BenhNhanViewDTO>
                {
                    PageIndex = searchModel.PageIndex,
                    PageSize = searchModel.PageSize,
                    TotalRecords = totalRecords,
                    Items = listView
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // --- HÀM PHỤ TRỢ (PRIVATE) ĐỂ CHUYỂN ĐỔI DỮ LIỆU ---
        // Viết 1 lần dùng cho tất cả các hàm trên cho gọn code
        private BenhNhanViewDTO MapToViewDTO(BenhNhan entity)
        {
            return new BenhNhanViewDTO
            {
                Id = entity.Id,
                HoTen = entity.HoTen,
                NgaySinh = entity.NgaySinh,
                GioiTinh = entity.GioiTinh,
                DiaChi = entity.DiaChi,
                SoTheBaoHiem = entity.SoTheBaoHiem,
                MucHuong = entity.MucHuong,
                HanTheBHYT = entity.HanTheBHYT,
                TrangThai = entity.TrangThai
            };
        }
    }
}