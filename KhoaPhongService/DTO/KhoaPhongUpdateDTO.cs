using System.ComponentModel.DataAnnotations;

namespace KhoaPhongService.DTO
{
    public class KhoaPhongUpdateDTO
    {
        [Required]
        public Guid Id { get; set; } // Bắt buộc phải có ID

        [Required(ErrorMessage = "Tên khoa không được để trống")]
        public string TenKhoa { get; set; }

        public string LoaiKhoa { get; set; }

        [Range(1, 1000)]
        public int SoGiuongTieuChuan { get; set; }
    }
}
