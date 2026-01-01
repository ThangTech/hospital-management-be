using System.ComponentModel.DataAnnotations;

namespace KhoaPhongService.DTO
{
    public class KhoaPhongCreateDTO
    {
        [Required(ErrorMessage = "Tên khoa không được để trống")]
        public string TenKhoa { get; set; }

        public string LoaiKhoa { get; set; }

        [Range(1, 1000, ErrorMessage = "Số giường phải lớn hơn 0")]
        public int SoGiuongTieuChuan { get; set; }
    }
}