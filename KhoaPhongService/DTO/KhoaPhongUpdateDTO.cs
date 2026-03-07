using System.ComponentModel.DataAnnotations;

namespace KhoaPhongService.DTO
{
    public class KhoaPhongUpdateDTO
    {
        [Required]
        public Guid Id { get; set; } // B?t bu?c ph?i có ID

        [Required(ErrorMessage = "Tên khoa không du?c d? tr?ng")]
        public string TenKhoa { get; set; }

        public string LoaiKhoa { get; set; }

        [Range(1, 1000)]
        public int SoGiuongTieuChuan { get; set; }
    }
}
