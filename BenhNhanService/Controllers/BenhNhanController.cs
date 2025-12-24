using BenhNhanService.BLL.Interfaces;
using QuanLyBenhNhan.Models;
using Microsoft.AspNetCore.Mvc;
using BenhNhanService.DTO;

namespace BenhNhanService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BenhNhanController : ControllerBase
    {
        private IBenhNhanBusiness _benhNhanBusiness;
        public BenhNhanController(IBenhNhanBusiness bus) { _benhNhanBusiness = bus; }

        // --- 1. CREATE: Trả về ViewDTO sau khi thêm ---
        [Route("create-benh-nhan")]
        [HttpPost]
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
                    SoTheBaoHiem = modelDto.SoTheBaoHiem
                };

                _benhNhanBusiness.Create(benhNhan);

                // CHUYỂN ĐỔI NGƯỢC LẠI SANG VIEW DTO ĐỂ TRẢ VỀ
                var resultDTO = MapToViewDTO(benhNhan);

                return StatusCode(201, new { Message = "Thêm thành công", Data = resultDTO });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // --- 2. UPDATE: Trả về ViewDTO sau khi sửa ---
        // Lưu ý: Tôi giữ nguyên logic ID bất biến như bạn yêu cầu
        [Route("update-benh-nhan")]
        [HttpPut]
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
                    SoTheBaoHiem = modelDto.SoTheBaoHiem
                };

                _benhNhanBusiness.Update(benhNhan);

                // CHUYỂN ĐỔI SANG VIEW DTO
                var resultDTO = MapToViewDTO(benhNhan);

                return Ok(new { Message = "Cập nhật thành công", Data = resultDTO });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // --- 3. DELETE: Hiển thị thông tin người vừa bị xóa ---
        [Route("delete")]
        [HttpPost]
        public IActionResult DeleteBenhNhan([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                string id = formData.ContainsKey("id") ? formData["id"].ToString() : "";

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
        [Route("get-by-id/{id}")]
        [HttpGet]
        public IActionResult GetDatabyID(string id)
        {
            var data = _benhNhanBusiness.GetDatabyID(id);
            if (data == null) return NotFound("Không tìm thấy");

            return Ok(MapToViewDTO(data));
        }

        // --- 5. GET ALL: Trả về List<ViewDTO> ---
        [Route("get-all")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var listEntity = _benhNhanBusiness.GetAll();
            // Dùng hàm Map cho cả danh sách
            var listView = listEntity.Select(x => MapToViewDTO(x)).ToList();
            return Ok(listView);
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
                SoTheBaoHiem = entity.SoTheBaoHiem
            };
        }

        // ... (Giữ hàm Search của bạn ở đây)
    }
}