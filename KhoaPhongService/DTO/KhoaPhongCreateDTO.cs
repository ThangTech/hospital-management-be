using System.ComponentModel.DataAnnotations;

namespace KhoaPhongService.DTO
{
    public class KhoaPhongCreateDTO
    {
        [Required(ErrorMessage = "Tên khoa không du?c d? tr?ng")]
        public string TenKhoa { get; set; }

        public string LoaiKhoa { get; set; }

        [Range(1, 1000, ErrorMessage = "S? giu?ng ph?i l?n hon 0")]
        public int SoGiuongTieuChuan { get; set; }
    }
}