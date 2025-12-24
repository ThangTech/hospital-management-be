using System.ComponentModel.DataAnnotations;

namespace BenhNhanService.DTO
{
    public class BenhNhanViewDTO
    {
        public Guid Id { get; set; }
        public string HoTen { get; set; }
        public DateOnly NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string SoTheBaoHiem { get; set; }
    }
    public class BenhNhanUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }
        public string HoTen { get; set; }
        public DateOnly NgaySinh { get; set; } // Dùng DateTime để hứng JSON cho dễ
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string SoTheBaoHiem { get; set; }
    }
}
